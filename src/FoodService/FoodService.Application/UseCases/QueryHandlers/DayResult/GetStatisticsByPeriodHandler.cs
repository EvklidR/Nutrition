using AutoMapper;
using FoodService.Application.DTOs.DayResult.Responses;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Queries.DayResult;
using FoodService.Domain.Interfaces;

namespace FoodService.Application.UseCases.QueryHandlers.DayResult
{
    public class GetDayResultsByPeriodQueryHandler 
        : IQueryHandler<GetStatisticsByPeriodQuery, IEnumerable<ShortDayResultResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetDayResultsByPeriodQueryHandler(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShortDayResultResponse>> Handle(
            GetStatisticsByPeriodQuery request,
            CancellationToken cancellationToken)
        {
            await _userService.CheckProfileBelongingAsync(
                request.UserId,
                request.ProfileId);

            var dayResults = await _unitOfWork.DayResultRepository.GetAllByParametersAsync(
                request.ProfileId, 
                paginatedParameters: null,
                request.PeriodParameters);

            var dayResultsDTO = _mapper.Map<List<ShortDayResultResponse>>(dayResults);

            return dayResultsDTO;
        }
    }
}
