using PostService.Core.Entities;
using PostService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using PostService.Infrastructure.MongoDB.Configurators;

namespace PostService.Infrastructure.MongoDB
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDBOptions> mongoSettings, PostConfigurator postConfigurator)
        {
            var settings = mongoSettings.Value;

            var client = new MongoClient(settings.ConnectionURI);
            
            _database = client.GetDatabase(settings.DatabaseName);

            postConfigurator.ConfigureIndexes(Posts);
        }

        public IMongoCollection<Post> Posts => _database.GetCollection<Post>("Posts");
    }
}