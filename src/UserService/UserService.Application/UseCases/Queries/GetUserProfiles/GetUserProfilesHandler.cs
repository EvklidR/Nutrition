using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.UseCases.Queries
{
    public class GetUserProfilesHandler : IQueryHandler<GetUserProfilesQuery, IEnumerable<Profile>?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserProfilesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Profile>?> Handle(
            GetUserProfilesQuery request,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProfileRepository.GetAllByUserAsync(request.userId);
        }
    }
}
