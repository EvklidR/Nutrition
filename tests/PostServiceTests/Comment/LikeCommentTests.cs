using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using PostService.BusinessLogic.Exceptions;
using PostService.Core.Entities;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostServiceTests
{
    public class LikeCommentTests
    {
        private readonly Faker _faker;
        private readonly Faker<Comment> _commentFaker;

        private readonly Mock<ICommentRepository> _commentRepositoryMock;

        private readonly PostService.BusinessLogic.Services.CommentService _commentService;

        public LikeCommentTests()
        {
            _faker = new Faker();

            _commentRepositoryMock = new Mock<ICommentRepository>();

            _commentFaker = new Faker<Comment>()
                .RuleFor(c => c.Id, f => f.Random.AlphaNumeric(24))
                .RuleFor(c => c.UserLikeIds, f => new List<string>());

            _commentService = new PostService.BusinessLogic.Services.CommentService(
                _commentRepositoryMock.Object,
                null,
                null);
        }

        [Fact]
        public async Task LikeCommentAsync_ShouldAddUserToLikes_WhenUserHasNotLikedComment()
        {
            // Arrange
            var comment = _commentFaker.Generate();
            var userId = _faker.Random.AlphaNumeric(24);

            _commentRepositoryMock.Setup(cr => cr.GetByIdAsync(comment.Id)).ReturnsAsync(comment);
            _commentRepositoryMock.Setup(cr => cr.UpdateAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);

            // Act
            await _commentService.LikeCommentAsync(comment.Id, userId);

            // Assert
            comment.UserLikeIds.Should().Contain(userId);

            _commentRepositoryMock.Verify(cr => cr.GetByIdAsync(comment.Id), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.UpdateAsync(comment), Times.Once);
        }

        [Fact]
        public async Task LikeCommentAsync_ShouldRemoveUserFromLikes_WhenUserHasAlreadyLikedComment()
        {
            // Arrange
            var userId = _faker.Random.AlphaNumeric(24);
            var comment = _commentFaker
                .RuleFor(c => c.UserLikeIds, f => new List<string> { userId })
                .Generate();

            _commentRepositoryMock.Setup(cr => cr.GetByIdAsync(comment.Id)).ReturnsAsync(comment);
            _commentRepositoryMock.Setup(cr => cr.UpdateAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);

            // Act
            await _commentService.LikeCommentAsync(comment.Id, userId);

            // Assert
            comment.UserLikeIds.Should().NotContain(userId);

            _commentRepositoryMock.Verify(cr => cr.GetByIdAsync(comment.Id), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.UpdateAsync(comment), Times.Once);
        }

        [Fact]
        public async Task LikeCommentAsync_ShouldThrowNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var commentId = _faker.Random.AlphaNumeric(24);
            var userId = _faker.Random.AlphaNumeric(24);

            _commentRepositoryMock.Setup(cr => cr.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Comment)null);

            // Act
            Func<Task> act = async () => await _commentService.LikeCommentAsync(commentId, userId);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Comment not found");

            _commentRepositoryMock.Verify(cr => cr.GetByIdAsync(commentId), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.UpdateAsync(It.IsAny<Comment>()), Times.Never);
        }
    }
}
