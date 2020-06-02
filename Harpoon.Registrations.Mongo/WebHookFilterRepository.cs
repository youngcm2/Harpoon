using System;
using MongoDB.Driver;

namespace Harpoon.Registrations.Mongo
{
    class WebHookFilterRepository : MongoContext<WebHookFilter, Guid>
    {
        public WebHookFilterRepository(IMongoDatabase database) : base(database)
        {
        }

        protected override string CollectionName => nameof(WebHookFilter);
        protected override Guid CreateNewId() => Guid.NewGuid();
    }
}