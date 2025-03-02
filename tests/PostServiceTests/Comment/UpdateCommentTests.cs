using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using PostService.BusinessLogic.DTOs.Comment;
using PostService.BusinessLogic.Exceptions;
using PostService.BusinessLogic.Mappers;
using PostService.Core.Entities;
using PostService.Infrastructure.Repositories.Interfaces;

namespace PostServiceTests
{
    public class UpdateCommentTests
    {
        private readonly Faker _faker;
        private readonly Faker<Comment> _commentFaker;
        private readonly Faker<UpdateCommentDTO> _updateCommentFaker;

        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly IMapper _mapper;

        private readonly PostService.BusinessLogic.Services.CommentService _commentService;

        public UpdateCommentTests()
        {
            _faker = new Faker();

            _commentRepositoryMock = new Mock<ICommentRepository>();

            _commentFaker = new Faker<Comment>()
                .RuleFor(c => c.Id, f => f.Random.AlphaNumeric(24))
                .RuleFor(c => c.OwnerId, f => f.Random.AlphaNumeric(24))
                .RuleFor(c => c.Text, f => f.Lorem.Sentence());

            _updateCommentFaker = new Faker<UpdateCommentDTO>()
                .RuleFor(dto => dto.Id, f => f.Random.AlphaNumeric(24))
                .RuleFor(dto => dto.Text, f => f.Lorem.Sentence());

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommentMappingProfile>();
            });
            _mapper = configuration.CreateMapper();

            _commentService = new PostService.BusinessLogic.Services.CommentService(
                _commentRepositoryMock.Object,
                null,
                _mapper);
        }

        [Fact]
        public async Task UpdateCommentAsync_ShouldUpdateComment_WhenUserIsOwner()
        {
            // Arrange
            var comment = _commentFaker.Generate();
            var updateCommentDTO = _updateCommentFaker.Generate();
            updateCommentDTO.Id = comment.Id;
            var userId = comment.OwnerId;

            _commentRepositoryMock.Setup(cr => cr.GetByIdAsync(comment.Id)).ReturnsAsync(comment);
            _commentRepositoryMock.Setup(cr => cr.UpdateAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);

            // Act
            await _commentService.UpdateCommentAsync(updateCommentDTO, userId);

            // Assert
            comment.Text.Should().Be(updateCommentDTO.Text);

            _commentRepositoryMock.Verify(cr => cr.GetByIdAsync(comment.Id), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.UpdateAsync(comment), Times.Once);
        }

        [Fact]
        public async Task UpdateCommentAsync_ShouldThrowNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var updateCommentDTO = _updateCommentFaker.Generate();
            var userId = _faker.Random.AlphaNumeric(24);

            _commentRepositoryMock.Setup(cr => cr.GetByIdAsync(updateCommentDTO.Id)).ReturnsAsync((Comment)null);

            // Act
            Func<Task> act = async () => await _commentService.UpdateCommentAsync(updateCommentDTO, userId);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Comment not found");

            _commentRepositoryMock.Verify(cr => cr.GetByIdAsync(updateCommentDTO.Id), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.UpdateAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCommentAsync_ShouldThrowForbidden_WhenUserIsNotOwner()
        {
            // Arrange
            var comment = _commentFaker.Generate();
            var updateCommentDTO = _updateCommentFaker.Generate();
            updateCommentDTO.Id = comment.Id;
            var userId = _faker.Random.AlphaNumeric(24); // Not the owner

            _commentRepositoryMock.Setup(cr => cr.GetByIdAsync(comment.Id)).ReturnsAsync(comment);

            // Act
            Func<Task> act = async () => await _commentService.UpdateCommentAsync(updateCommentDTO, userId);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You don't have permission to do it");

            _commentRepositoryMock.Verify(cr => cr.GetByIdAsync(comment.Id), Times.Once);
            _commentRepositoryMock.Verify(cr => cr.UpdateAsync(It.IsAny<Comment>()), Times.Never);
        }
    }
}
