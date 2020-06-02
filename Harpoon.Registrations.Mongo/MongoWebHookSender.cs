using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Harpoon.Sender;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Harpoon.Registrations.Mongo
{
    /// <summary>
    /// <see cref="IWebHookSender"/> implementation that automatically pauses webhooks on NotFound responses
    /// </summary>
    public class MongoWebHookSender : DefaultWebHookSender
    {
        private readonly IMongoContext<WebHook> _context;
        private readonly IMongoContext<WebHookLog> _logContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoWebHookSender"/> class.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="signatureService"></param>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        /// <param name="logContext"></param>
        public MongoWebHookSender(HttpClient httpClient, ISignatureService signatureService, ILogger<DefaultWebHookSender> logger, IMongoContext<WebHook> context, IMongoContext<WebHookLog> logContext )
            : base(httpClient, signatureService, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logContext = logContext;
        }

        /// <inheritdoc />
        protected override Task OnFailureAsync(HttpResponseMessage response, Exception exception, IWebHookWorkItem webHookWorkItem, CancellationToken cancellationToken)
            => AddLogAsync(webHookWorkItem, $"WebHook {webHookWorkItem.WebHook.Id} failed. [{webHookWorkItem.WebHook.Callback}]: {exception.Message}");

        /// <inheritdoc />
        protected override Task OnSuccessAsync(HttpResponseMessage response, IWebHookWorkItem webHookWorkItem, CancellationToken cancellationToken)
            => AddLogAsync(webHookWorkItem);

        /// <inheritdoc />
        protected override async Task OnNotFoundAsync(HttpResponseMessage response, IWebHookWorkItem webHookWorkItem, CancellationToken cancellationToken)
        {
            var filter = Builders<WebHook>.Filter.Where(w => w.Id == webHookWorkItem.WebHook.Id);
            var dbWebHook = await _context.Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            if (dbWebHook != null)
            {
                dbWebHook.IsPaused = true;
            }

            await AddLogAsync(webHookWorkItem, $"WebHook {webHookWorkItem.WebHook.Id} was paused. [{webHookWorkItem.WebHook.Callback}]");
        }

        private async Task AddLogAsync(IWebHookWorkItem workItem, string error = null)
        {
            var log = new WebHookLog
            {
                Error = error,
                WebHookId = workItem.WebHook.Id,
                WebHookNotificationId = workItem.Id
            };
            try
            {
                await _logContext.SaveAsync(log);

                if (!string.IsNullOrEmpty(error))
                {
                    Logger.LogInformation(error);
                }
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.LogError(error);
                }

                Logger.LogError($"Log failed for WebHook {workItem.WebHook.Id}. [{workItem.WebHook.Callback}]: {e.Message}");
            }
        }
    }
}