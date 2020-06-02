using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Harpoon.Registrations.Mongo
{
    /// <summary>
    /// <see cref="IQueuedProcessor{IWebHookNotification}"/> implementation that logs everything into the context
    /// </summary>
    public class MongoNotificationProcessor : DefaultNotificationProcessor
    {
        private readonly IMongoContext<WebHookNotification> _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoNotificationProcessor"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webHookStore"></param>
        /// <param name="webHookSender"></param>
        /// <param name="logger"></param>
        public MongoNotificationProcessor(IMongoContext<WebHookNotification> context, IWebHookStore webHookStore, IWebHookSender webHookSender, ILogger<DefaultNotificationProcessor> logger)
            : base(webHookStore, webHookSender, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        protected override async Task<Guid> LogAsync(IWebHookNotification notification, IReadOnlyList<IWebHook> webHooks, CancellationToken cancellationToken)
        {
            var notif = new WebHookNotification
            {
                Payload = notification.Payload,
                TriggerId = notification.TriggerId,
                Count = webHooks.Count
            };
            await _context.SaveAsync(notif);
            
            return notif.Id;
        }
    }
}