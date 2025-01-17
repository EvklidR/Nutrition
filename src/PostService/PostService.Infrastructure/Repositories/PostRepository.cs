using MongoDB.Driver;
using PostService.Core.Entities;
using PostService.Infrastructure.MongoDB;
using PostService.Infrastructure.Repositories.Interfaces;
using PostService.Infrastructure.Repositories.Projections;

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

        public async Task<(IEnumerable<PostDTO>?, long totalCount)> GetAllAsync(List<string>? words, int? page, int? size, string userId)
        {
            var filter = Builders<Post>.Filter.Empty;

            if (words != null && words.Any())
            {
                filter = Builders<Post>.Filter.Text(string.Join(" ", words));
            }

            var query = _posts
                .Find(filter)
                .Project(post => new PostDTO
                {
                    Id = post.Id.ToString(),
                    Name = post.Name,
                    Text = post.Text,
                    KeyWords = post.KeyWords,
                    AmountOfLikes = post.UserLikeIds.Count,
                    AmountOfComments = post.Comments.Count,
                    IsLiked = post.UserLikeIds.Contains(userId)
                });

            var totalCount = await _posts.CountDocumentsAsync(filter);

            if (page.HasValue && size.HasValue)
            {
                query = query.Skip((page.Value - 1) * size.Value).Limit(size.Value);
            }

            return (await query.ToListAsync(), totalCount);
        }



        public async Task AddPostAsync(Post post)
        {
            await _posts.InsertOneAsync(post);
        }

        public async Task DeletePostAsync(string postId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            await _posts.DeleteOneAsync(filter);
        }

        public async Task UpdatePostAsync(string postId, Post updatedPost)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            var update = Builders<Post>.Update
                .Set(p => p.Name, updatedPost.Name)
                .Set(p => p.Text, updatedPost.Text)
                .Set(p => p.KeyWords, updatedPost.KeyWords)
                .Set(p => p.UserLikeIds, updatedPost.UserLikeIds);

            var result = await _posts.UpdateOneAsync(filter, update);
        }
    }
}
