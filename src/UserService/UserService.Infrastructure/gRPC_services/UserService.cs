using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Grpc;
using UserService.Infrastructure.MSSQL;

namespace UserService.Infrastructure.gRPC
{
    public class UserService : GRPCUserService.GRPCUserServiceBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public UserService(UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async override Task<CheckUserResponse> CheckUser(CheckUserRequest request, ServerCallContext context)
        {
            if (Guid.TryParse(request.UserId, out Guid userId)) 
            {
                var existUser = await _userManager.FindByIdAsync(userId.ToString());

                return new CheckUserResponse
                {
                    Exists = existUser != null
                };
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "User id is't Guid type"));
            }
        }

        public async override Task<CheckProfileBelongingResponse> CheckProfileBelonging(CheckProfileBelongingRequest request, ServerCallContext context)
        {
            if (Guid.TryParse(request.UserId, out Guid userId) && Guid.TryParse(request.ProfileId, out Guid profileId))
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return new CheckProfileBelongingResponse()
                    {
                        Belong = false
                    };
                }

                var profile = await _dbContext.Profiles.FirstOrDefaultAsync(profile => profile.Id == profileId);

                if (profile == null)
                {
                    return new CheckProfileBelongingResponse()
                    {
                        Belong = false
                    };
                }

                return new CheckProfileBelongingResponse()
                {
                    Belong = profile.UserId == userId
                };
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Ids aren't Guid type"));
            }
        }
    }
}