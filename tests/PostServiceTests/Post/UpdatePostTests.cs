using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using PostService.BusinessLogic.DTOs.Post;
using PostService.BusinessLogic.Exceptions;
using PostService.BusinessLogic.Mappers;
using PostService.Core.Entities;
using PostService.Infrastructure.Repositories.Interfaces;
using PostService.Infrastructure.Services.Interfaces;

namespace PostServiceTests
{
    public class UpdatePostTests
    {
        private readonly Faker _faker;
        private readonly Faker<UpdatePostDTO> _updatePostFaker;
        private readonly Faker<Post> _postFaker;

        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IImageService> _imageServiceMock;

        private readonly IMapper _mapper;
        private readonly PostService.BusinessLogic.Services.PostService _postService;

        public UpdatePostTests()
        {
            _faker = new Faker();

            _postRepositoryMock = new Mock<IPostRepository>();
            _imageServiceMock = new Mock<IImageService>();

            _postFaker = new Faker<Post>()
                .RuleFor(p => p.Id, f => f.Random.AlphaNumeric(24))
                .RuleFor(p => p.OwnerId, f => f.Random.AlphaNumeric(24))
                .RuleFor(p => p.Text, f => f.Lorem.Paragraph())
                .RuleFor(p => p.UserLikeIds, f => new List<string>());

            _updatePostFaker = new Faker<UpdatePostDTO>()
                .RuleFor(dto => dto.Id, f => f.Random.AlphaNumeric(24))
                .RuleFor(dto => dto.Text, f => f.Lorem.Paragraph())
                .RuleFor(dto => dto.Files, f => new List<Microsoft.AspNetCore.Http.IFormFile>());

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PostMappingProfile>();
            });
            _mapper = configuration.CreateMapper();

            _postService = new PostService.BusinessLogic.Services.PostService(
                _postRepositoryMock.Object,
                _mapper,
                null,
                _imageServiceMock.Object);
        }

        [Fact]
        public async Task UpdatePostAsync_ShouldUpdatePost_WhenPostExistsAndUserIsOwner()
        {
            // Arrange
            var post = _postFaker.Generate();
            var postDTO = _updatePostFaker.Generate();
            postDTO.Id = post.Id;
            var userId = post.OwnerId;

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(post.Id)).ReturnsAsync(post);
            _postRepositoryMock.Setup(pr => pr.UpdateAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);

            // Act
            await _postService.UpdatePostAsync(postDTO, userId);

            // Assert
            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(post.Id), Times.Once);
            _postRepositoryMock.Verify(pr => pr.UpdateAsync(It.IsAny<Post>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePostAsync_ShouldThrowNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var postDTO = _updatePostFaker.Generate();
            var userId = _faker.Random.AlphaNumeric(24);

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(postDTO.Id)).ReturnsAsync((Post)null);

            // Act
            Func<Task> act = async () => await _postService.UpdatePostAsync(postDTO, userId);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Post not found");

            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(postDTO.Id), Times.Once);
            _postRepositoryMock.Verify(pr => pr.UpdateAsync(It.IsAny<Post>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePostAsync_ShouldThrowForbidden_WhenUserIsNotOwner()
        {
            // Arrange
            var post = _postFaker.Generate();
            var postDTO = _updatePostFaker.Generate();
            postDTO.Id = post.Id;
            var userId = _faker.Random.AlphaNumeric(24);

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(post.Id)).ReturnsAsync(post);

            // Act
            Func<Task> act = async () => await _postService.UpdatePostAsync(postDTO, userId);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You don't have permission to this action");

            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(post.Id), Times.Once);
            _postRepositoryMock.Verify(pr => pr.UpdateAsync(It.IsAny<Post>()), Times.Never);
        }
    }
}
