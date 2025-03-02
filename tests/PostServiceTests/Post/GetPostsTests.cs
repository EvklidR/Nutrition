using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using PostService.BusinessLogic.Mappers;
using PostService.Core.Entities;
using PostService.Infrastructure.gRPC.Interfaces;
using PostService.Infrastructure.Repositories.Interfaces;
using PostService.Infrastructure.Services.Interfaces;

namespace PostServiceTests
{
    public class GetPostsTests
    {
        private readonly Faker _faker;
        private readonly Faker<Post> _postFaker;

        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly IMapper _mapper;

        private readonly PostService.BusinessLogic.Services.PostService _postService;

        public GetPostsTests()
        {
            _faker = new Faker();

            _postRepositoryMock = new Mock<IPostRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PostMappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();

            _postFaker = new Faker<Post>()
                .RuleFor(x => x.Id, f => f.Random.AlphaNumeric(24));

            _postService = new PostService.BusinessLogic.Services.PostService(
                _postRepositoryMock.Object,
                _mapper,
                null,
                null);
        }

        [Fact]
        public async Task PostService_ShouldRetutnPosts()
        {
            //Arrange
            var posts = _postFaker.Generate(5);
            var userId = _faker.Random.AlphaNumeric(24);

            _postRepositoryMock.Setup(pr => pr.GetAllAsync(It.IsAny<List<string>?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync((posts, posts.Count));

            //Act
            var response = await _postService.GetPostsAsync(new List<string>(), 1, 5, userId);

            //Assert
            response.TotalCount.Should().Be(posts.Count);
            response?.Posts?.Select(x => x.Id).Should().BeEquivalentTo(posts.Select(x => x.Id));

            _postRepositoryMock.Verify(pr => pr.GetAllAsync(It.IsAny<List<string>?>(), It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
        }
    }
}
