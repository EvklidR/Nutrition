using UserService.Application.Exceptions;
using UserService.Domain.Interfaces;

namespace UserService.Application.UseCases.Commands
{
    public class RevokeTokenHandler : ICommandHandler<RevokeTokenCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RevokeTokenHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.userId);
            if (user == null) throw new Unauthorized("User not found");

            user.RefreshToken = null;
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
