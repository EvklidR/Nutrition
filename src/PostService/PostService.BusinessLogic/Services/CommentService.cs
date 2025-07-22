using AutoMapper;
using PostService.BusinessLogic.DTOs.Requests.Comment;
using PostService.BusinessLogic.DTOs.Responses.Comment;
using PostService.BusinessLogic.Exceptions;
using PostService.Core.Entities;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostService.BusinessLogic.Services
{
    public class CommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<List<CommentResponse>> GetCommentsAsync(string postId, int? page, int? size, string userId)
        {
            var comments = await _commentRepository.GetAllAsync(postId, page, size);

            var commentsDTO = _mapper.Map<List<CommentResponse>>(comments, opts =>
            {
                opts.Items["CurrentUserId"] = userId;
            });

            return commentsDTO;
        }

        public async Task<CommentResponse> AddCommentAsync(CreateCommentDTO commentDTO, string ownerEmail, string ownerId)
        {
            var post = await _postRepository.GetByIdAsync(commentDTO.PostId);

            if (post == null)
            {
                throw new NotFound("Post not found");
            }

            var comment = _mapper.Map<Comment>(commentDTO);
            comment.OwnerEmail = ownerEmail;
            comment.OwnerId = ownerId;

            await _commentRepository.AddAsync(commentDTO.PostId, comment);

            return _mapper.Map<CommentResponse>(comment, opts =>
            {
                opts.Items["CurrentUserId"] = ownerId;
            });
        }

        public async Task DeleteCommentAsync(string commentId, string userId) 
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

            _mapper.Map(commentDTO, comment);

            await _commentRepository.UpdateAsync(comment);
        }

        public async Task LikeCommentAsync(string commentId, string userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                throw new NotFound("Comment not found");
            }

            if (comment.UserLikeIds.Contains(userId))
            {
                comment.UserLikeIds.Remove(userId);
            }
            else
            {
                comment.UserLikeIds.Add(userId);
            }

            await _commentRepository.UpdateAsync(comment);
        }
    }
}
