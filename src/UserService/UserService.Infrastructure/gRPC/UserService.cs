using Grpc.Core;
using MediatR;
using UserService.Application.UseCases.Queries;
using UserService.Grpc;

namespace UserService.Infrastructure.gRPC
{
    public class UserService : GRPCUserService.GRPCUserServiceBase
    {
        private readonly IMediator _mediator;
        public UserService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<CheckUserResponse> CheckUser(CheckUserRequest request, ServerCallContext context)
        {
            var responce = new CheckUserResponse();

            if (Guid.TryParse(request.UserId, out Guid userId)) 
            {
                responce.Exists = await _mediator.Send(new CheckUserByIdQuery(userId));
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "User id is't Guid type"));
            }
            return responce;
        }

        public async override Task<CheckProfileBelongingResponse> CheckProfileBelonging(CheckProfileBelongingRequest request, ServerCallContext context)
        {
            if (Guid.TryParse(request.UserId, out Guid userId) && Guid.TryParse(request.ProfileId, out Guid profileId))
            {
                return new CheckProfileBelongingResponse()
                {
                    Belong = await _mediator.Send(new CheckProfileBelongingQuery(userId, profileId))
                };
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Ids aren't Guid type"));
            }
        }

        public async override Task<CheckProfileBelongingResponse> CheckProfileBelonging(CheckProfileBelongingRequest request, ServerCallContext context)
        {
            if (Guid.TryParse(request.UserId, out Guid userId) && Guid.TryParse(request.ProfileId, out Guid profileId))
            {
                return new CheckProfileBelongingResponse()
                { 
                    Belong = await _mediator.Send(new CheckProfileBelongingQuery(userId, profileId)) 
                };
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Ids aren't Guid type"));
            }
        }
    }
}