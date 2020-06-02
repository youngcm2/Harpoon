using System;
using MongoDB.Driver;

namespace Harpoon.Registrations.Mongo
{
    class WebHookNotificationRepository : MongoContext<WebHookNotification, Guid>
    {
        public WebHookNotificationRepository(IMongoDatabase database) : base(database)
        {
        }

        protected override string CollectionName => nameof(WebHookNotification);
        protected override Guid CreateNewId()=>Guid.NewGuid();
    }
}