using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Commands.DayResult;
using FoodService.Application.DTOs.DayResult.Responses;

namespace FoodService.Application.UseCases.CommandHandlers.DayResult;

public class CreateDayResultHandler 
    : ICommandHandler<CreateDayResultCommand, DayResultResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public CreateDayResultHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserService checkUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userService = checkUserService;
    }

    public async Task<DayResultResponse> Handle(
        CreateDayResultCommand request,
        CancellationToken cancellationToken)
    {
        await _userService.CheckProfileBelongingAsync(
            request.UserId, 
            request.CreateDayResultDTO.ProfileId);

        var dayResult = _mapper.Map<Domain.Entities.DayResult>(request.CreateDayResultDTO);

        _unitOfWork.DayResultRepository.Add(dayResult);

        await _unitOfWork.SaveChangesAsync();

        var dayResultDTO = _mapper.Map<DayResultResponse>(dayResult);

        return dayResultDTO;
    }
}
