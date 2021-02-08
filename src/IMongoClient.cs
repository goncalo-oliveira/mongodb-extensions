using MongoDB.Driver;

namespace Faactory.Extensions.MongoDB
{
    public interface IMongoService
    {
        IMongoClient Client { get; }
    }
}
