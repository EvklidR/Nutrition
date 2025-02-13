using Bogus;
using FluentAssertions;
using Moq;
using PostService.BusinessLogic.Exceptions;
using PostService.Core.Entities;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostServiceTests
{
    public class DeleteCommentTests
    {
        private readonly Faker _faker;

        private readonly Mock<ICommentRepository> _commentRepositoryMock;

        private readonly PostService.BusinessLogic.Services.CommentService _commentService;

        public DeleteCommentTests()
        {
            _faker = new Faker();

            _commentRepositoryMock = new Mock<ICommentRepository>();

            _commentService = new PostService.BusinessLogic.Services.CommentService(
                _commentRepositoryMock.Object,
                null,
                null);
        }

        [Fact]
        public async Task DeleteCommentAsync_ShouldDeleteComment()
        {
            // Arrange
            var commentId = _faker.Random.AlphaNumeric(24);
            var userId = _faker.Random.AlphaNumeric(24);
            var comment = new Comment { Id = commentId, OwnerId = userId };

            _commentRepositoryMock.Setup(cr => cr.GetByIdAsync(commentId)).ReturnsAsync(comment);
            _commentRepositoryMock.Setup(cr => cr.DeleteAsync(commentId)).Returns(Task.CompletedTask);

            // Act
            await _commentService.DeleteCommentAsync(commentId, userId);

            // Assert
            _commentRepositoryMock.Verify(cr => cr.GetByIdAsync(commentId), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.DeleteAsync(commentId), Times.Once);
        }

        [Fact]
        public async Task DeleteCommentAsync_ShouldThrowNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var commentId = _faker.Random.AlphaNumeric(24);
            var userId = _faker.Random.AlphaNumeric(24);

            _commentRepositoryMock.Setup(cr => cr.GetByIdAsync(commentId)).ReturnsAsync((Comment)null);

            // Act
            Func<Task> act = async () => await _commentService.DeleteCommentAsync(commentId, userId);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Comment not found");

            _commentRepositoryMock.Verify(cr => cr.GetByIdAsync(commentId), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCommentAsync_ShouldThrowForbidden_WhenUserIsNotOwner()
        {
            // Arrange
            var commentId = _faker.Random.AlphaNumeric(24);
            var ownerId = _faker.Random.AlphaNumeric(24);
            var userId = _faker.Random.AlphaNumeric(24);
            var comment = new Comment { Id = commentId, OwnerId = ownerId };

            _commentRepositoryMock.Setup(cr => cr.GetByIdAsync(commentId)).ReturnsAsync(comment);

            // Act
            Func<Task> act = async () => await _commentService.DeleteCommentAsync(commentId, userId);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You don't have permission to do it");

            _commentRepositoryMock.Verify(cr => cr.GetByIdAsync(commentId), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.DeleteAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
