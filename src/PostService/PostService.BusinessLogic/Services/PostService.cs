using AutoMapper;
using PostService.BusinessLogic.DTOs.Post;
using PostService.BusinessLogic.Exceptions;
using PostService.BusinessLogic.Models;
using PostService.Core.Entities;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostService.BusinessLogic.Services
{
    public class PostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
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

        public async Task CreatePostAsync(CreatePostDTO postDTO)
        {
            var post = _mapper.Map<Post>(postDTO);
            await _postRepository.AddAsync(post);
        }

        public async Task DeletePostAsync(string postId)
        {
            await _postRepository.DeleteAsync(postId);
        }

        public async Task UpdatePostAsync(UpdatePostDTO postDTO)
        {
            var post = await _postRepository.GetByIdAsync(postDTO.Id);
            if (post == null)
            {
                throw new NotFound("Post not found");
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
