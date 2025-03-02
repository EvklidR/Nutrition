using Microsoft.AspNetCore.Identity;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Application.UseCases.Queries
{
    public class CheckProfileBelongingHandler : IQueryHandler<CheckProfileBelongingQuery, bool>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly UserManager<User> _userManager;

        public CheckProfileBelongingHandler(IProfileRepository profileRepository, UserManager<User> userManager)
        {
            _profileRepository = profileRepository;
            _userManager = userManager;
        }

        public async Task<bool> Handle(CheckProfileBelongingQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.userId.ToString());

            if (user == null)
            {
                return false;
            }

            var profile = await _profileRepository.GetByIdAsync(request.profileId);

            if (profile == null)
            {
                return false;
            }

            return profile.UserId.Equals(request.userId);
        }
    }
}
