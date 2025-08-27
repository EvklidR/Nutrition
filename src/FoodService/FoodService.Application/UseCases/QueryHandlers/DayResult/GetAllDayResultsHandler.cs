using AutoMapper;
using FoodService.Application.DTOs.DayResult.Responses;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Queries.DayResult;
using FoodService.Domain.Interfaces;

namespace FoodService.Application.UseCases.QueryHandlers.DayResult;

public class GetAllDayResultsHandler : IQueryHandler<GetAllDayResultsQuery, DayResultsResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public GetAllDayResultsHandler(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<DayResultsResponse> Handle(GetAllDayResultsQuery request, CancellationToken cancellationToken)
    {
        await _userService.CheckProfileBelongingAsync(
                request.UserId,
                request.ProfileId);

        var (dayResults, totalCount) = await _unitOfWork.DayResultRepository.GetAllByParametersAsync(
            request.ProfileId,
            request.PaginationParameters,
            request.PeriodParameters);

        var dayResultsDTO = _mapper.Map<List<ShortDayResultResponse>>(dayResults);

        var response = new DayResultsResponse
        {
            DayResults = dayResultsDTO,
            TotalCount = totalCount
        };

        return response;
    }
}
