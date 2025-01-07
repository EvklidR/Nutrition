using Moq;
using UserService.Application.UseCases.Commands;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserServiceTests
{
    public class RevokeTokenTests
    {
        private readonly Mock<IRefreshTokenTokenRepository> _tokenRepositoryMock;
        private readonly RevokeTokenHandler _handler;

        public RevokeTokenTests()
        {
            _tokenRepositoryMock = new Mock<IRefreshTokenTokenRepository>();
            _handler = new RevokeTokenHandler(_tokenRepositoryMock.Object);
        }

        [Fact]
        public async Task Should_Revoke_Token_Successfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tokenToRevoke = new RefreshToken { Token = "valid-refresh-token", UserId = userId };
            var tokens = new List<RefreshToken> { tokenToRevoke };

            _tokenRepositoryMock.Setup(repo => repo.GetAllByUserAsync(userId))
                .ReturnsAsync(tokens);

            var command = new RevokeTokenCommand(userId, "valid-refresh-token");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _tokenRepositoryMock.Verify(repo => repo.Delete(It.Is<RefreshToken>(t => t.Token == "valid-refresh-token")), Times.Once);
            _tokenRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
