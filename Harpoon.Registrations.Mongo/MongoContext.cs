using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Harpoon.Registrations.Mongo
{
    abstract class MongoContext<TDocument, TId> : IMongoContext<TDocument>
        where TDocument : BaseDocument<TId> where TId : IComparable<TId>
    {
        private readonly IMongoDatabase _database;

        protected MongoContext(IMongoDatabase database)
        {
            _database = database;
        }

        private IMongoCollection<TDocument> _collection;

        protected abstract string CollectionName { get; }

        public IMongoCollection<TDocument> Collection =>
            _collection ??= _database.GetCollection<TDocument>(CollectionName);
        protected abstract TId CreateNewId();

        public async Task<TDocument> SaveAsync(TDocument item)
        {
            if (Equals(item.Id, default(TId)))
            {
                item.Id = CreateNewId();
                await Collection.InsertOneAsync(item);
            }
            else
            {
                var filter = Builders<TDocument>.Filter.Eq("_id", item.Id);
                await Collection.ReplaceOneAsync(filter, item, new ReplaceOptions {IsUpsert = true});
            }

            return item;
        }
        
        
    }
}