using PostService.Core.Entities;

namespace PostService.Infrastructure.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(string postId);
        Task<IEnumerable<Post>?> GetAllAsync(List<string>? words, int? page, int? size, string userId);
        Task AddPostAsync(Post post);
        Task DeletePostAsync(string postId);
        Task UpdatePostAsync(string postId, Post updatedPost);
    }
}
