using UserService.Contracts.Broker;
using UserService.Contracts.Broker.Enums;
using UserService.Contracts.DataAccess.Repositories;
using UserService.Contracts.Exceptions;

namespace UserService.Application.UseCases.Commands;

public class DeleteProfileHandler : ICommandHandler<DeleteProfileCommand>
{
    private readonly IProfileRepository _profileRepository;
    private readonly IBrokerService _brokerService;

    public DeleteProfileHandler(
        IProfileRepository profileRepository,
        IBrokerService brokerService)
    {
        _profileRepository = profileRepository;
        _brokerService = brokerService;
    }

    public async Task Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepository.GetByIdAsync(request.ProfileId, cancellationToken);

        if (profile == null)
        {
            throw new NotFound("Profile not found");
        }

        if (request.UserId != profile!.UserId)
        {
            throw new Unauthorized("Owner isn't valid");
        }

        await _profileRepository.DeleteAsync(profile, cancellationToken);

        await _brokerService.PublishMessageAsync(
            profile.Id.ToString(), 
            queueName: null, 
            exchange: ExchangeName.ProfileDeleted, 
            cancellationToken: cancellationToken);
    }
}
