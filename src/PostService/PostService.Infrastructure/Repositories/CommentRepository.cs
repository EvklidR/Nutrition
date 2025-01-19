using MongoDB.Driver;
using PostService.Core.Entities;
using PostService.Infrastructure.MongoDB;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostService.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Post> _posts;
        public CommentRepository(MongoDBContext context)
        {
            _posts = context.Posts;
        }

        public async Task<Comment?> GetByIdAsync(Guid commentId)
        {
            var filter = Builders<Post>.Filter.ElemMatch(p => p.Comments, c => c.Id == commentId);
            var post = await _posts.Find(filter).FirstOrDefaultAsync();

            return post?.Comments.FirstOrDefault(c => c.Id == commentId);
        }

        public async Task<IEnumerable<Comment>?> GetAllAsync(string postId, int? page, int? size)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            var post = await _posts.Find(filter).FirstOrDefaultAsync();

            if (post == null)
            {
                return null;
            }

            var comments = post.Comments.AsEnumerable();

            if (page.HasValue && size.HasValue)
            {
                comments = comments.Skip((page.Value - 1) * size.Value).Take(size.Value);
            }

            return comments;
        }

        public async Task AddAsync(string postId, Comment comment)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            var update = Builders<Post>.Update.Push(p => p.Comments, comment);

            await _posts.UpdateOneAsync(filter, update);
        }

        public async Task UpdateAsync(Comment updatedComment)
        {
            var filter = Builders<Post>.Filter.ElemMatch(p => p.Comments, c => c.Id == updatedComment.Id);
            var update = Builders<Post>.Update.Set("Comments.$", updatedComment);

            await _posts.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(Guid commentId)
        {
            var filter = Builders<Post>.Filter.ElemMatch(p => p.Comments, c => c.Id == commentId);
            var update = Builders<Post>.Update.PullFilter(p => p.Comments, c => c.Id == commentId);

            await _posts.UpdateOneAsync(filter, update);
        }
    }
}
