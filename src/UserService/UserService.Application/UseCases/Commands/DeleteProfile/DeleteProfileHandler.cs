using UserService.Application.Exceptions;
using UserService.Domain.Interfaces;

namespace UserService.Application.UseCases.Commands
{
    public class DeleteProfileHandler : ICommandHandler<DeleteProfileCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProfileHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.ProfileRepository.GetByIdAsync(request.profileId);

            if (profile == null)
                throw new NotFound("Profile not found.");

            if (request.userId != profile!.UserId)
                throw new Unauthorized("Owner isn't valid");

            _unitOfWork.ProfileRepository.Delete(profile);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
