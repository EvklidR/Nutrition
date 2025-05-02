using AutoMapper;
using Microsoft.AspNetCore.Http;
using PostService.BusinessLogic.DTOs.Post;
using PostService.BusinessLogic.Exceptions;
using PostService.BusinessLogic.Models;
using PostService.Core.Entities;
using PostService.Infrastructure.gRPC.Interfaces;
using PostService.Infrastructure.Repositories.Interfaces;
using PostService.Infrastructure.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PostService.BusinessLogic.Services
{
    public class PostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public PostService(
            IPostRepository postRepository,
            IMapper mapper,
            IUserService userService, 
            IImageService imageService)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _userService = userService;
            _imageService = imageService;
        }

        public async Task<PostsResponseModel> GetPostsAsync(
            List<string>? words,
            int? page,
            int? size,
            string userId)
        {
            var response = await _postRepository.GetAllAsync(words, page, size);

            var postsDTO = _mapper.Map<List<PostDTO>>(response.posts, opts =>
            {
                opts.Items["CurrentUserId"] = userId;
            });

            return new PostsResponseModel
            {
                Posts = postsDTO,
                TotalCount = response.totalCount
            };
        }

        public async Task<PostDTO> GetPostAsync(
            string postId,
            string userId)
        {
            var response = await _postRepository.GetByIdAsync(postId);

            if (response == null)
            {
                throw new NotFound("Post not found");
            }

            var postDTO = _mapper.Map<PostDTO>(response, opts =>
            {
                opts.Items["CurrentUserId"] = userId;
            });

            return postDTO;
        }

        public async Task<PostsResponseModel> GetUserPostsAsync(int? page, int? size, string userId)
        {
            var response = await _postRepository.GetByUserIdAsync(userId, page, size);

            var postsDTO = _mapper.Map<List<PostDTO>>(response.posts, opts =>
            {
                opts.Items["CurrentUserId"] = userId;
            });

            return new PostsResponseModel() 
            { 
                Posts = postsDTO,
                TotalCount = response.totalCount
            };
        }

        public async Task<PostDTO> CreatePostAsync(CreatePostDTO postDTO)
        {
            var userExists = await _userService.CheckUserExistence(postDTO.OwnerId!);

            if (!userExists)
            {
                throw new Unauthorized("There is no user with this id");
            }

            var markdown = await UploadImagesAsync(postDTO.Files, postDTO.Text);

            postDTO.Text = markdown;

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

            var imageLinks = ExtractLinks(post.Text);

            await DeleteImagesAsync(imageLinks);

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

            var markdown = await UploadImagesAsync(postDTO.NewFiles, postDTO.Text);

            var oldImageLinks = ExtractLinks(post.Text);

            var newImageLinks = ExtractLinks(postDTO.Text);

            List<string> linksToDelete = new List<string>();

            foreach(var link in oldImageLinks)
            {
                if (!newImageLinks.Contains(link))
                {
                    linksToDelete.Add(link);
                }
            }

            await DeleteImagesAsync(linksToDelete);

            post = _mapper.Map(postDTO, post);

            await _postRepository.UpdateAsync(post);
        }

        public async Task LikePostAsync(string postId, string userId)
        {
            var post = await _postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                throw new NotFound("Post not found");
            }

            if (post.UserLikeIds.Contains(userId))
            {
                post.UserLikeIds.Remove(userId);
            }
            else
            {
                post.UserLikeIds.Add(userId);
            }

            await _postRepository.UpdateAsync(post);
        }
        
        private async Task<string> UploadImagesAsync(List<IFormFile> files, string markdownText)
        {
            var fileNameToUrlMap = new Dictionary<string, string>();

            foreach (var file in files)
            {
                var isFileValid = file.Length > 0 && markdownText.Contains(file.FileName);

                if (isFileValid)
                {
                    var extention = file.FileName.Split('.').Last();

                    var dropboxPath = $"/Posts/{Guid.NewGuid()}.{extention}";

                    using (var stream = file.OpenReadStream())
                    {
                        var success = await _imageService.UploadImageAsync(stream, dropboxPath);

                        if (!success)
                        {
                            throw new BadRequest("Failed to load image");
                        }

                        fileNameToUrlMap[file.FileName] = dropboxPath;
                    }
                }
            }

            foreach (var entry in fileNameToUrlMap)
            {
                var fileName = entry.Key;
                var imageUrl = entry.Value;

                var searchPattern = $"![]({fileName})";
                var replacement = $"![Image]({imageUrl})";

                markdownText = markdownText.Replace(searchPattern, replacement);
            }

            return markdownText;
        }

        private async Task DeleteImagesAsync(List<string> imageLinks)
        {
            foreach (var link in imageLinks)
            {
                if (!string.IsNullOrEmpty(link))
                {
                    var success = await _imageService.DeleteImageAsync(link);

                    if (!success)
                    {
                        throw new BadRequest("Failed to delete image");
                    }
                }
            }
        }

        public async Task<Stream> GetImageAsync(string imageName)
        {
            string decodedUrl = Uri.UnescapeDataString(imageName);

            var imageStream = await _imageService.DownloadImageAsync(decodedUrl);    
            
            if (imageStream == null)
            {
                throw new NotFound($"Image {decodedUrl} not found");
            }

            return imageStream;
        }

        private List<string> ExtractLinks(string text)
        {
            return Regex.Matches(text, @"!\[Image\]\((.*?)\)")
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToList();
        }
    }
}
