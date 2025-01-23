using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using PostService.BusinessLogic.Exceptions;
using PostService.Core.Entities;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostServiceTests
{
    public class LikePostTests
    {
        private readonly Faker _faker;
        private readonly Faker<Post> _postFaker;

        private readonly Mock<IPostRepository> _postRepositoryMock;

        private readonly PostService.BusinessLogic.Services.PostService _postService;

        public LikePostTests()
        {
            _faker = new Faker();

            _postRepositoryMock = new Mock<IPostRepository>();

            _postFaker = new Faker<Post>()
                .RuleFor(x => x.Id, f => f.Random.AlphaNumeric(24))
                .RuleFor(x => x.UserLikeIds, f => new List<string>());

            _postService = new PostService.BusinessLogic.Services.PostService(
                _postRepositoryMock.Object,
                null,
                null,
                null);
        }

        [Fact]
        public async Task LikePostAsync_ShouldAddUserToLikes_WhenUserHasNotLikedPost()
        {
            // Arrange
            var post = _postFaker.Generate();
            var userId = _faker.Random.AlphaNumeric(24);

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(post.Id)).ReturnsAsync(post);
            _postRepositoryMock.Setup(pr => pr.UpdateAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);

            // Act
            await _postService.LikePostAsync(post.Id, userId);

            // Assert
            post.UserLikeIds.Should().Contain(userId);

            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(post.Id), Times.Once);
            _postRepositoryMock.Verify(pr => pr.UpdateAsync(post), Times.Once);
        }

        [Fact]
        public async Task LikePostAsync_ShouldRemoveUserFromLikes_WhenUserHasAlreadyLikedPost()
        {
            // Arrange
            var userId = _faker.Random.AlphaNumeric(24);
            var post = _postFaker
                .RuleFor(x => x.UserLikeIds, f => new List<string> { userId })
                .Generate();

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(post.Id)).ReturnsAsync(post);
            _postRepositoryMock.Setup(pr => pr.UpdateAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);

            // Act
            await _postService.LikePostAsync(post.Id, userId);

            // Assert
            post.UserLikeIds.Should().NotContain(userId);

            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(post.Id), Times.Once);
            _postRepositoryMock.Verify(pr => pr.UpdateAsync(post), Times.Once);
        }

        [Fact]
        public async Task LikePostAsync_ShouldThrowNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = _faker.Random.AlphaNumeric(24);
            var userId = _faker.Random.AlphaNumeric(24);

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Post)null);

            // Act
            Func<Task> act = async () => await _postService.LikePostAsync(postId, userId);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Post not found");

            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(postId), Times.Once);
            _postRepositoryMock.Verify(pr => pr.UpdateAsync(It.IsAny<Post>()), Times.Never);
        }
    }
}
