using System.Threading.Tasks;
using MongoDB.Driver;

namespace Harpoon.Registrations.Mongo
{
    public interface IMongoContext<TContext>
    {
        IMongoCollection<TContext> Collection { get; }
        Task<TContext> SaveAsync(TContext item);
    }
}