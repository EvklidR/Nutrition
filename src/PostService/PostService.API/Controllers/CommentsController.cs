using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.API.Filters;
using PostService.BusinessLogic.DTOs.Comment;
using PostService.BusinessLogic.DTOs.Post;
using PostService.BusinessLogic.Services;

namespace PostService.API.Controllers
{
    /// <summary>
    /// Controller fo managing comments of posts.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentsController(CommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Retrieves a list of comments for a specified post.
        /// </summary>
        /// <param name="postId">The ID of the post.</param>
        /// <param name="page">The page number (optional).</param>
        /// <param name="size">The number of comments per page (optional).</param>
        /// <returns>A list of comments.</returns>
        [HttpGet("{postId}")]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(List<CommentDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComments(string postId, [FromQuery] int? page = 1, int? size = 10)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            var comments = await _commentService.GetCommentsAsync(postId, page, size, userId);

            return Ok(comments);
        }

        /// <summary>
        /// Adds a new comment to a post.
        /// </summary>
        /// <param name="createCommentDTO">The data required to create a comment.</param>
        /// <returns>Created comment.</returns>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDTO createCommentDTO)
        {
            var userId = (string)HttpContext.Items["UserId"]!;
            var userName = (string)HttpContext.Items["UserName"]!;

            createCommentDTO.OwnerId = userId;
            createCommentDTO.OwnerEmail = userName;

            var responce = await _commentService.AddCommentAsync(createCommentDTO);

            return Ok(responce);
        }

        /// <summary>
        /// Deletes a comment by its ID.
        /// </summary>
        /// <param name="commentId">The ID of the comment to delete.</param>
        [HttpDelete("{commentId:guid}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            await _commentService.DeleteCommentAsync(commentId, userId);

            return NoContent();
        }

        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="updateCommentDTO">The data required to update the comment.</param>
        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDTO updateCommentDTO)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            await _commentService.UpdateCommentAsync(updateCommentDTO, userId);

            return NoContent();
        }

        /// <summary>
        /// Likes a comment by its ID.
        /// </summary>
        /// <param name="commentId">The ID of the comment to like.</param>
        [HttpPost("{postId}/like")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> LikePost(string commentId)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            await _commentService.LikeCommentAsync(commentId, userId);

            return NoContent();
        }
    }
}
