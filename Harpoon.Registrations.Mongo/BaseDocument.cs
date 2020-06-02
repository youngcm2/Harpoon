using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Harpoon.Registrations.Mongo
{
    public abstract class GuidBaseDocument: BaseDocument<Guid>
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public override Guid Id { get; set; }
    }

    public abstract class BaseDocument<TId> where TId : IComparable<TId>
    {
        protected BaseDocument()
        {
        }

        [BsonIgnore]
        public abstract TId Id { get; set; }
    }
    
    public abstract class ObjectIdBaseDocument: BaseDocument<ObjectId>
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public override ObjectId Id { get; set; }
    }
}