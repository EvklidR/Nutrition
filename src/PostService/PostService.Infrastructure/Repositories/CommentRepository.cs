using MongoDB.Driver;
using PostService.Core.Entities;
using PostService.Infrastructure.MongoDB;
using PostService.Infrastructure.Repositories.Interfaces;
using PostService.Infrastructure.Repositories.Projections;

namespace PostService.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Post> _posts;
        public CommentRepository(MongoDBContext context)
        {
            _posts = context.Posts;
        }

        public async Task<IEnumerable<CommentDTO>?> GetAllAsync(string postId, int? page, int? size, string userId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            var post = await _posts.Find(filter).FirstOrDefaultAsync();

            if (post == null)
            {
                return null;
            }

            var comments = post.Comments.Select(comment => new CommentDTO
            {
                Id = comment.Id,
                OwnerEmail = comment.OwnerEmail,
                OwnerId = comment.OwnerId,
                Date = comment.Date,
                Text = comment.Text,
                AmountOfLikes = comment.UserLikeIds.Count,
                IsLiked = comment.UserLikeIds.Contains(userId)
            });

            if (page.HasValue && size.HasValue)
            {
                comments = comments.Skip((page.Value - 1) * size.Value).Take(size.Value);
            }

            return comments;
        }


        public async Task AddCommentAsync(string postId, Comment comment)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            var update = Builders<Post>.Update.Push(p => p.Comments, comment);

            await _posts.UpdateOneAsync(filter, update);
        }

        public async Task UpdateCommentAsync(Comment updatedComment)
        {
            var filter = Builders<Post>.Filter.ElemMatch(p => p.Comments, c => c.Id == updatedComment.Id);
            var update = Builders<Post>.Update.Set("Comments.$", updatedComment);

            await _posts.UpdateOneAsync(filter, update);
        }

        public async Task DeleteCommentAsync(string postId, Guid commentId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            var update = Builders<Post>.Update.PullFilter(p => p.Comments, c => c.Id == commentId);

            await _posts.UpdateOneAsync(filter, update);
        }
    }
}
