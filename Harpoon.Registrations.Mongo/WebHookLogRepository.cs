using MongoDB.Bson;
using MongoDB.Driver;

namespace Harpoon.Registrations.Mongo
{
    class WebHookLogRepository : MongoContext<WebHookLog, ObjectId>
    {
        public WebHookLogRepository(IMongoDatabase database) : base(database)
        {
        }

        protected override string CollectionName => nameof(WebHookLog);
        protected override ObjectId CreateNewId()=>ObjectId.GenerateNewId();
        
    }
}