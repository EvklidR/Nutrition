using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using PostService.BusinessLogic.DTOs.Requests.Comment;
using PostService.BusinessLogic.Exceptions;
using PostService.BusinessLogic.Mappers;
using PostService.Core.Entities;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostServiceTests
{
    public class AddCommentTests
    {
        private readonly Faker _faker;
        private readonly Faker<CreateCommentDTO> _createCommentFaker;
        private readonly Faker<Comment> _commentFaker;

        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<ICommentRepository> _commentRepositoryMock;

        private readonly IMapper _mapper;
        private readonly PostService.BusinessLogic.Services.CommentService _commentService;

        public AddCommentTests()
        {
            _faker = new Faker();

            _postRepositoryMock = new Mock<IPostRepository>();
            _commentRepositoryMock = new Mock<ICommentRepository>();

            _commentFaker = new Faker<Comment>()
                .RuleFor(c => c.Id, f => f.Random.AlphaNumeric(24))
                .RuleFor(c => c.OwnerId, f => f.Random.AlphaNumeric(24))
                .RuleFor(c => c.Text, f => f.Lorem.Sentence());

            _createCommentFaker = new Faker<CreateCommentDTO>()
                .RuleFor(dto => dto.PostId, f => f.Random.AlphaNumeric(24))
                .RuleFor(dto => dto.Text, f => f.Lorem.Sentence())
                .RuleFor(dto => dto.OwnerId, f => f.Random.AlphaNumeric(24));

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommentMappingProfile>();
            });
            _mapper = configuration.CreateMapper();

            _commentService = new PostService.BusinessLogic.Services.CommentService(
                _commentRepositoryMock.Object,
                _postRepositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task AddCommentAsync_ShouldAddComment_WhenPostExists()
        {
            // Arrange
            var postId = _faker.Random.AlphaNumeric(24);
            var post = new Post { Id = postId };
            var createCommentDTO = _createCommentFaker.Generate();
            createCommentDTO.PostId = postId;

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(postId)).ReturnsAsync(post);
            _commentRepositoryMock.Setup(cr => cr.AddAsync(postId, It.IsAny<Comment>())).Returns(Task.CompletedTask);

            // Act
            var result = await _commentService.AddCommentAsync(createCommentDTO);

            // Assert
            result.Should().NotBeNull();
            result.Text.Should().Be(createCommentDTO.Text);

            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(postId), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.AddAsync(postId, It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task AddCommentAsync_ShouldThrowNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var createCommentDTO = _createCommentFaker.Generate();

            _postRepositoryMock.Setup(pr => pr.GetByIdAsync(createCommentDTO.PostId)).ReturnsAsync((Post)null);

            // Act
            Func<Task> act = async () => await _commentService.AddCommentAsync(createCommentDTO);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Post not found");

            _postRepositoryMock.Verify(pr => pr.GetByIdAsync(createCommentDTO.PostId), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.AddAsync(It.IsAny<string>(), It.IsAny<Comment>()), Times.Never);
        }
    }
}
