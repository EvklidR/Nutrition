﻿using PostService.Core.Entities;

namespace PostService.Infrastructure.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(string commentId);
        Task<IEnumerable<Comment>?> GetAllAsync(string postId, int? page, int? size);
        Task AddAsync(string postId, Comment comment);
        Task UpdateAsync(Comment updatedComment);
        Task DeleteAsync(string commentId);
    }
}
