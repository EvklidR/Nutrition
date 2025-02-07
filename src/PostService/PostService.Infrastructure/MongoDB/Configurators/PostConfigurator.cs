using MongoDB.Driver;
using PostService.Core.Entities;

namespace PostService.Infrastructure.MongoDB.Configurators
{
    public class PostConfigurator
    {
        public void ConfigureIndexes(IMongoCollection<Post> posts)
        {
            var indexKeys = Builders<Post>.IndexKeys
                .Text(x => x.Title)
                .Text(x => x.Text)
                .Text(x => x.KeyWords);

            posts.Indexes.CreateOne(new CreateIndexModel<Post>(indexKeys));
        }
    }
}
