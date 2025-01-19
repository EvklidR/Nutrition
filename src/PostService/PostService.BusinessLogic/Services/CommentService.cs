using AutoMapper;
using PostService.BusinessLogic.DTOs.Comment;
using PostService.BusinessLogic.Exceptions;
using PostService.Core.Entities;
using PostService.Infrastructure.Repositories.Interfaces;
using System.ComponentModel.Design;

namespace PostService.BusinessLogic.Services
{
    public class CommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository commentRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task AddCommentAsync(CreateCommentDTO commentDTO)
        {
            var comment = _mapper.Map<Comment>(commentDTO);

            await _commentRepository.AddAsync(commentDTO.PostId, comment);
        }

        public async Task DeleteCommentAsync(Guid commentId, string userId) 
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null) 
            {
                throw new NotFound("Comment not found");
            }

            if (comment.OwnerId != userId)
            {
                throw new Forbidden("You don't have permission to do it");
            }

            await _commentRepository.DeleteAsync(commentId);
        }

        public async Task UpdateCommentAsync(UpdateCommentDTO commentDTO, string userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentDTO.Id);
            if (comment == null)
            {
                throw new NotFound("Comment not found");
            }

            if (comment.OwnerId != userId)
            {
                throw new Forbidden("You don't have permission to do it");
            }
        }
    }
}
