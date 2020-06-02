using System;
using MongoDB.Driver;

namespace Harpoon.Registrations.Mongo
{
    class WebHookRepository : MongoContext<WebHook, Guid>
    {
        public WebHookRepository(IMongoDatabase database) : base(database)
        {
        }

        protected override string CollectionName => nameof(WebHook);
        protected override Guid CreateNewId() => Guid.NewGuid();
    }
}