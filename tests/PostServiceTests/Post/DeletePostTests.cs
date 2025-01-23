using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using PostService.BusinessLogic.Exceptions;
using PostService.Core.Entities;
using PostService.Infrastructure.gRPC.Interfaces;
using PostService.Infrastructure.Repositories.Interfaces;
using PostService.Infrastructure.Services.Interfaces;

namespace PostServiceTests
{
    public class DeletePostTests
    {
        private readonly Faker _faker;
        private readonly Faker<Post> _postFaker;

        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IImageService> _imageServiceMock;

        private readonly PostService.BusinessLogic.Services.PostService _postService;

        public DeletePostTests()
        {
            _faker = new Faker();

            _postRepositoryMock = new Mock<IPostRepository>();
            _imageServiceMock = new Mock<IImageService>();

            _postFaker = new Faker<Post>()
                .RuleFor(x => x.Id, f => f.Random.AlphaNumeric(24))
                .RuleFor(x => x.OwnerId, f => f.Random.AlphaNumeric(24))
                .RuleFor(x => x.Text, f => f.Lorem.Paragraph());

            _postService = new PostService.BusinessLogic.Services.PostService(
                _postRepositoryMock.Object,
                null,
                null,
                _imageServiceMock.Object);
        }

        [Fact]
        public async Task PostService_ShouldDeletePost_WhenPostExists()
        {
            //Arrange
            Post post = _postFaker.Generate();

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(post.Id)).ReturnsAsync(post);
            _postRepositoryMock.Setup(pr => pr.DeleteAsync(post.Id)).Returns(Task.CompletedTask);

            //Act
            await _postService.DeletePostAsync(post.Id, post.OwnerId);

            //Assert
            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(post.Id), Times.Once);
            _postRepositoryMock.Verify(pr => pr.DeleteAsync(post.Id), Times.Once);
        }

        [Fact]
        public async Task PostService_ShouldThrowException_WhenPostNotFound()
        {
            //Arrange
            var postId = _faker.Random.AlphaNumeric(24);

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Post)null);

            //Act
            Func<Task> act = async() => await _postService.DeletePostAsync(postId, _faker.Random.AlphaNumeric(24));

            //Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Post not found");

            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(postId), Times.Once);
            _postRepositoryMock.Verify(pr => pr.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task PostService_ShouldThrowException_WhenUserIdDoesntMatches()
        {
            //Arrange
            Post post = _postFaker.Generate();
            var userId = _faker.Random.AlphaNumeric(24);

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(post);

            //Act
            Func<Task> act = async () => await _postService.DeletePostAsync(post.Id, userId);

            //Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You don't have permission to this action");

            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(post.Id), Times.Once);
            _postRepositoryMock.Verify(pr => pr.DeleteAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
