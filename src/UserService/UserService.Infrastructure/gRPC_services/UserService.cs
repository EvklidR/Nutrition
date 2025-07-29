using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using UserService.Grpc;
using UserService.Infrastructure.MSSQL;

namespace UserService.Infrastructure.gRPC;

public class UserService : GRPCUserService.GRPCUserServiceBase
{
    private readonly ApplicationDbContext _dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async override Task<CheckUserResponse> CheckUser(CheckUserRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.UserId, out Guid userId)) 
        {
            var isUserExists = await _dbContext.Users.AnyAsync(user => user.Id == userId);

            return new CheckUserResponse
            {
                Exists = isUserExists
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
            var isUserExists = await _dbContext.Users.AnyAsync(user => user.Id == userId);

            if (!isUserExists)
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

    public async override Task<GetProfileWeightResponse> GetProfileWeight(GetProfileWeightRequest request, ServerCallContext context)
    {
        if (Guid.TryParse(request.ProfileId, out Guid profileId))
        {
            var profile = await _dbContext.Profiles.FirstOrDefaultAsync(profile => profile.Id == profileId);

            if (profile == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Profile not found"));
            }

            return new GetProfileWeightResponse()
            {
                Weight = profile.Weight
            };
        }
        else
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Profile Id isn't Guid type"));
        }
    }
}