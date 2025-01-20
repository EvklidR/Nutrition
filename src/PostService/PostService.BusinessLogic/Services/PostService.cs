using AutoMapper;
using PostService.BusinessLogic.DTOs.Post;
using PostService.BusinessLogic.Exceptions;
using PostService.BusinessLogic.Models;
using PostService.Core.Entities;
using PostService.Infrastructure.gRPC.Interfaces;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostService.BusinessLogic.Services
{
    public class PostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public PostService(IPostRepository postRepository, IMapper mapper, IUserService userService)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<PostsResponseModel> GetPostsAsync(List<string>? words, int? page, int? size, string userId)
        {
            var response = await _postRepository.GetAllAsync(words, page, size);

            var postsDTO = _mapper.Map<List<PostDTO>>(response.posts, opts =>
            {
                opts.Items["CurrentUserId"] = userId;
            });

            return new PostsResponseModel() { Posts = postsDTO, TotalCount = response.totalCount };
        }

        public async Task<PostsResponseModel> GetUserPostsAsync(int? page, int? size, string userId)
        {
            var response = await _postRepository.GetByUserIdAsync(userId, page, size);

            var postsDTO = _mapper.Map<List<PostDTO>>(response.posts, opts =>
            {
                opts.Items["CurrentUserId"] = userId;
            });

            return new PostsResponseModel() { Posts = postsDTO, TotalCount = response.totalCount };
        }

        public async Task<PostDTO> CreatePostAsync(CreatePostDTO postDTO)
        {
            var userExists = await _userService.CheckUserExistence(postDTO.OwnerId!);

            if (!userExists)
            {
                throw new Unauthorized("There is no user with this id");
            }

            var post = _mapper.Map<Post>(postDTO);

            await _postRepository.AddAsync(post);

            return _mapper.Map<PostDTO>(post, opts =>
            {
                opts.Items["CurrentUserId"] = postDTO.OwnerId;
            });
        }

        public async Task DeletePostAsync(string postId, string userId)
        {
            var post = await _postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                throw new NotFound("Post not found");
            }

            if (post.OwnerId != userId)
            {
                throw new Forbidden("You don't have permission to this action");
            }

            await _postRepository.DeleteAsync(postId);
        }

        public async Task UpdatePostAsync(UpdatePostDTO postDTO, string userId)
        {
            var post = await _postRepository.GetByIdAsync(postDTO.Id);

            if (post == null)
            {
                throw new NotFound("Post not found");
            }

            if (post.OwnerId != userId)
            {
                throw new Forbidden("You don't have permission to this action");
            }

            post = _mapper.Map(postDTO, post);

            await _postRepository.UpdateAsync(post);
        }

        public async Task LikePost(string postId, string userId)
        {
            var post = await _postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                throw new NotFound("Post not found");
            }

            post.UserLikeIds.Add(userId);

            await _postRepository.UpdateAsync(post);
        }
    }
}
