using MongoDB.Driver;
using PostService.Core.Entities;
using PostService.Infrastructure.MongoDB;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostService.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IMongoCollection<Post> _posts;

        public PostRepository(MongoDBContext context)
        {
            _posts = context.Posts;
        }

        public async Task<Post?> GetByIdAsync(string postId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            var post = await _posts.Find(filter).FirstOrDefaultAsync();
            return post;
        }

        public async Task<(IEnumerable<Post>? posts, long totalCount)> GetAllAsync(List<string>? words, int? page, int? size)
        {
            var filter = Builders<Post>.Filter.Empty;

            if (words != null && words.Any())
            {
                filter = Builders<Post>.Filter.Text(string.Join(" ", words));
            }

            var query = _posts.Find(filter);

            var totalCount = await _posts.CountDocumentsAsync(filter);

            if (page.HasValue && size.HasValue)
            {
                query = query.Skip((page.Value - 1) * size.Value).Limit(size.Value);
            }

            return (await query.ToListAsync(), totalCount);
        }

        public async Task AddAsync(Post post)
        {
            await _posts.InsertOneAsync(post);
        }

        public async Task DeleteAsync(string postId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            await _posts.DeleteOneAsync(filter);
        }

        public async Task UpdateAsync(Post updatedPost)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, updatedPost.Id);
            var update = Builders<Post>.Update
                .Set(p => p.Name, updatedPost.Name)
                .Set(p => p.Text, updatedPost.Text)
                .Set(p => p.KeyWords, updatedPost.KeyWords)
                .Set(p => p.UserLikeIds, updatedPost.UserLikeIds);

            var result = await _posts.UpdateOneAsync(filter, update);
        }
    }
}
