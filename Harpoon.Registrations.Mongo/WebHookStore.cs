using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Harpoon.Registrations.EFStorage;
using MongoDB.Driver;

namespace Harpoon.Registrations.Mongo
{
    /// <summary>
    /// Default <see cref="IWebHookStore"/> implementation using EF
    /// </summary>
    public class WebHookStore : IWebHookStore
    {
        
        private readonly ISecretProtector _secretProtector;
        private  readonly IMongoContext<WebHook> _context;

        public WebHookStore(IMongoContext<WebHook> context, ISecretProtector secretProtector)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _secretProtector = secretProtector ?? throw new ArgumentNullException(nameof(secretProtector));
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<IWebHook>> GetApplicableWebHooksAsync(IWebHookNotification notification,
            CancellationToken cancellationToken = default)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }
            
            var filterDefinition = Builders<WebHook>.Filter.Where(w => !w.IsPaused && w.Filters.Count == 0 || w.Filters.Any(f => f.Trigger == notification.TriggerId));

            var webHooks = await _context.Collection.Find(filterDefinition).ToListAsync(cancellationToken);

            foreach (var webHook in webHooks)
            {
                webHook.Secret = _secretProtector.Unprotect(webHook.ProtectedSecret);
            }
            return webHooks;
        }
    }
}