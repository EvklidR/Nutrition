using PostService.Core.Entities;

namespace PostService.Infrastructure.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(string postId);
        Task<(IEnumerable<Post>? posts, long totalCount)> GetByUserIdAsync(string userId, int? page, int? size);
        Task<(IEnumerable<Post>? posts, long totalCount)> GetAllAsync(List<string>? words, int? page, int? size);
        Task AddAsync(Post post);
        Task DeleteAsync(string postId);
        Task UpdateAsync(Post updatedPost);
    }
}
