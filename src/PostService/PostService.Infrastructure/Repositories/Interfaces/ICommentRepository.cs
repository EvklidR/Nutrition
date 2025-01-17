using PostService.Core.Entities;

namespace PostService.Infrastructure.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>?> GetAllAsync(string postId, int? page, int? size);
        Task AddCommentAsync(string postId, Comment comment);
        Task UpdateCommentAsync(Comment updatedComment);
        Task DeleteCommentAsync(string postId, Guid commentId);
    }
}
