using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PostService.Core.Entities;
using PostService.Infrastructure.Extentions.IFindFluentExtensions;
using PostService.Infrastructure.Extentions.IQueriableExtensions;
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
            try
            {
                var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);

                var post = await _posts.Find(filter).FirstOrDefaultAsync();

                return post;
            }
            catch
            {
                return null;
            }
        }

        public async Task<(IEnumerable<Post>? posts, long totalCount)> GetAllByUserIdAsync(string userId, int? page, int? size)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.OwnerId, userId);

            var totalCount = await _posts.CountDocumentsAsync(filter);

            var posts = await _posts.AsQueryable()
                .Where(p => p.OwnerId == userId)
                .GetPaginated(page, size)
                .ToListAsync();

            return (posts, totalCount);
        }

        public async Task<(IEnumerable<Post>? posts, long totalCount)> GetAllAsync(List<string>? words, int? page, int? size)
        {
            var filter = Builders<Post>.Filter.Empty;

            if (words != null && words.Any())
            {
                filter = Builders<Post>.Filter.Text(string.Join(" ", words), "russian");
            }
            
            var totalCount = await _posts.CountDocumentsAsync(filter);

            var posts = await _posts.Find(filter).GetPaginated(page, size).ToListAsync();

            return (posts, totalCount);
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

            var result = await _posts.ReplaceOneAsync(filter, updatedPost);
        }
    }
}
