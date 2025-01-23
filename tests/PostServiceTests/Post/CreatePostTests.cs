using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using PostService.BusinessLogic.DTOs.Post;
using PostService.BusinessLogic.Exceptions;
using PostService.Core.Entities;
using PostService.Infrastructure.gRPC.Interfaces;
using PostService.Infrastructure.Repositories.Interfaces;
using PostService.Infrastructure.Services.Interfaces;

namespace PostServiceTests
{
    public class CreatePostTests
    {
        private readonly Faker _faker;
        private readonly Faker<CreatePostDTO> _createPostDTOFaker;
        private readonly Faker<Post> _postFaker;

        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IImageService> _imageServiceMock;

        private readonly PostService.BusinessLogic.Services.PostService _postService;

        public CreatePostTests()
        {
            _faker = new Faker();

            _postRepositoryMock = new Mock<IPostRepository>();
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();
            _imageServiceMock = new Mock<IImageService>();

            _createPostDTOFaker = new Faker<CreatePostDTO>()
                .RuleFor(x => x.OwnerId, f => f.Random.AlphaNumeric(24))
                .RuleFor(x => x.Text, f => f.Lorem.Paragraph(40));

            _postFaker = new Faker<Post>()
                .RuleFor(x => x.Id, f => f.Random.AlphaNumeric(24));

            _postService = new PostService.BusinessLogic.Services.PostService(
                _postRepositoryMock.Object,
                _mapperMock.Object, 
                _userServiceMock.Object, 
                _imageServiceMock.Object);
        }

        [Fact]
        public async Task PostService_ShouldCreatePost()
        {
            //Arrange
            CreatePostDTO createPostDTO = _createPostDTOFaker.Generate();
            Post post = _postFaker.Generate();
            PostDTO postDTO = new PostDTO()
            {
                Id = post.Id
            };

            _userServiceMock.Setup(us => us.CheckUserExistence(createPostDTO.OwnerId!)).ReturnsAsync(true);

            _mapperMock.Setup(m => m.Map<Post>(createPostDTO)).Returns(post);
            _mapperMock.Setup(m => m.Map<PostDTO>(post, It.IsAny<Action<IMappingOperationOptions<object, PostDTO>>>()))
                .Returns(postDTO);

            _postRepositoryMock.Setup(pr => pr.AddAsync(post)).Returns(Task.CompletedTask);

            //Act
            var result = await _postService.CreatePostAsync(createPostDTO);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(post.Id);

            _userServiceMock.Verify(us => us.CheckUserExistence(createPostDTO.OwnerId!), Times.Once);
            _mapperMock.Verify(m => m.Map<Post>(createPostDTO), Times.Once);
            _mapperMock.Verify(m => m.Map<PostDTO>(post, It.IsAny<Action<IMappingOperationOptions<object, PostDTO>>>()), Times.Once);
            _postRepositoryMock.Verify(pr => pr.AddAsync(post), Times.Once);
        }

        [Fact]
        public async Task PostService_ShouldThrowUnauthorise_WhenUserDoesntExist()
        {
            //Arrange
            CreatePostDTO createPostDTO = _createPostDTOFaker.Generate();

            _userServiceMock.Setup(us => us.CheckUserExistence(createPostDTO.OwnerId!)).ReturnsAsync(false);

            //Act
            Func<Task> act = async () => await _postService.CreatePostAsync(createPostDTO);

            //Assert
            await act.Should().ThrowAsync<Unauthorized>()
                .WithMessage("There is no user with this id");
        }
    }
}
