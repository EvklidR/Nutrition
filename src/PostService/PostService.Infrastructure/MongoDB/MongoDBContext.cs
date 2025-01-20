using PostService.Core.Entities;
using PostService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace PostService.Infrastructure.MongoDB
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDBOptions> mongoSettings)
        {
            var settings = mongoSettings.Value;
            var client = new MongoClient(settings.ConnectionURI);
            
            _database = client.GetDatabase(settings.DatabaseName);

            var indexKeys = Builders<Post>.IndexKeys
                .Text(x => x.Title)
                .Text(x => x.Text)
                .Text(x => x.KeyWords);

            Posts.Indexes.CreateOne(new CreateIndexModel<Post>(indexKeys));
        }

        public IMongoCollection<Post> Posts => _database.GetCollection<Post>("Posts");
    }
}