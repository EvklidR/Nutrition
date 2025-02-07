using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using PostService.BusinessLogic.Mappers;
using PostService.Core.Entities;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostServiceTests
{
    public class GetCommentsTests
    {
        private readonly Faker _faker;
        private readonly Faker<Comment> _commentFaker;

        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly IMapper _mapper;

        private readonly PostService.BusinessLogic.Services.CommentService _commentService;

        public GetCommentsTests()
        {
            _faker = new Faker();

            _commentRepositoryMock = new Mock<ICommentRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommentMappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();

            _commentFaker = new Faker<Comment>()
                .RuleFor(c => c.Id, f => f.Random.AlphaNumeric(24))
                .RuleFor(c => c.OwnerId, f => f.Random.AlphaNumeric(24))
                .RuleFor(c => c.Text, f => f.Lorem.Sentence());

            _commentService = new PostService.BusinessLogic.Services.CommentService(
                _commentRepositoryMock.Object,
                null,
                _mapper);
        }

        [Fact]
        public async Task GetCommentsAsync_ShouldReturnComments_WhenCommentsExist()
        {
            // Arrange
            var postId = _faker.Random.AlphaNumeric(24);
            var userId = _faker.Random.AlphaNumeric(24);
            var comments = _commentFaker.Generate(5);

            _commentRepositoryMock
                .Setup(cr => cr.GetAllAsync(postId, It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(comments);

            // Act
            var result = await _commentService.GetCommentsAsync(postId, 1, 5, userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(comments.Count);
            result.Select(c => c.Id).Should().BeEquivalentTo(comments.Select(c => c.Id));

            _commentRepositoryMock.Verify(cr => cr.GetAllAsync(postId, It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
        }

        [Fact]
        public async Task GetCommentsAsync_ShouldReturnEmptyList_WhenNoCommentsExist()
        {
            // Arrange
            var postId = _faker.Random.AlphaNumeric(24);
            var userId = _faker.Random.AlphaNumeric(24);

            _commentRepositoryMock
                .Setup(cr => cr.GetAllAsync(postId, It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(new List<Comment>());

            // Act
            var result = await _commentService.GetCommentsAsync(postId, 1, 5, userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _commentRepositoryMock.Verify(cr => cr.GetAllAsync(postId, It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
        }
    }
}
