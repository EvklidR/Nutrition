using Microsoft.AspNetCore.Identity;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Queries
{
    public class CheckUserByIdHandler : IQueryHandler<CheckUserByIdQuery, bool>
    {
        private readonly UserManager<User> _userManager;

        public CheckUserByIdHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(CheckUserByIdQuery request, CancellationToken cancellationToken)
        {
            var existUser = await _userManager.FindByIdAsync(request.userId.ToString());
            return existUser != null;
        }
    }
}