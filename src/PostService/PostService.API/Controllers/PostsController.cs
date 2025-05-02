using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.API.Filters;
using PostService.BusinessLogic.DTOs.Post;
using PostService.BusinessLogic.Models;

namespace PostService.API.Controllers
{
    /// <summary>
    /// Controller fo managing posts.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly BusinessLogic.Services.PostService _postService;

        public PostsController(BusinessLogic.Services.PostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// Retrieves a list of posts based on search keywords, optionally with pagination.
        /// </summary>
        /// <param name="words">Search keywords (optional).</param>
        /// <param name="page">The page number (optional).</param>
        /// <param name="size">The number of posts per page (optional).</param>
        /// <returns>A list of posts and the total count.</returns>
        [HttpGet]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(PostsResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<PostsResponseModel>> GetPosts([FromQuery] List<string>? words = null, int? page = 1, int? size = 10)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            var response = await _postService.GetPostsAsync(words, page, size, userId);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a post based on post id.
        /// </summary>
        /// <param name="postId">Post id.</param>
        /// <returns>A list of posts and the total count.</returns>
        [HttpGet("{postId}")]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(PostDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<PostDTO>> GetPost(string postId)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            var response = await _postService.GetPostAsync(postId, userId);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves all posts created by a specific user.
        /// </summary>
        /// <param name="page">The page number (optional).</param>
        /// <param name="size">The number of posts per page (optional).</param>
        /// <returns>A list of posts of user and the total count.</returns>
        [HttpGet("by_user")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(PostsResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<PostsResponseModel>> GetUserPosts(
            [FromQuery] int? page = 1,
            [FromQuery] int? size = 10)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            var response = await _postService.GetUserPostsAsync(page, size, userId);

            return Ok(response);
        }

        /// <summary>
        /// Creates a new post.
        /// </summary>
        /// <param name="createPostDTO">The data required to create the post.</param>
        /// <returns>DTO of new instance.</returns>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(PostDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<PostDTO>> CreatePost([FromForm] CreatePostDTO createPostDTO)
        {
            var userId = (string)HttpContext.Items["UserId"]!;
            var userName = (string)HttpContext.Items["UserName"]!;

            createPostDTO.OwnerId = userId;
            createPostDTO.OwnerEmail = userName;

            var response = await _postService.CreatePostAsync(createPostDTO);

            return Ok(response);
        }

        /// <summary>
        /// Deletes a post by its ID.
        /// </summary>
        /// <param name="postId">The ID of the post to delete.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{postId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            await _postService.DeletePostAsync(postId, userId);

            return NoContent();
        }

        /// <summary>
        /// Updates an existing post.
        /// </summary>
        /// <param name="updatePostDTO">The data required to update the post.</param>
        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePost([FromForm] UpdatePostDTO updatePostDTO)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            await _postService.UpdatePostAsync(updatePostDTO, userId);

            return NoContent();
        }

        /// <summary>
        /// Likes a post by its ID.
        /// </summary>
        /// <param name="postId">The ID of the post to like.</param>
        [HttpPost("{postId}/like")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> LikePost(string postId)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            await _postService.LikePostAsync(postId, userId);

            return NoContent();
        }

        /// <summary>
        /// Get image from post.
        /// </summary>
        /// <param name="fileName">The path to image.</param>
        [HttpGet("/{fileName}")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<FileStreamResult>> GetFileAsync(string fileName)
        {
            var fileStream = await _postService.GetImageAsync(fileName);

            return File(fileStream, "image/jpeg");
        }
    }
}
