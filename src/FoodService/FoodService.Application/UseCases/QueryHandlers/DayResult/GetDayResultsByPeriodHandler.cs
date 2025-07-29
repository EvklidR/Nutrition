using AutoMapper;
using FoodService.Application.DTOs.DayResult.Responses;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Queries.DayResult;
using FoodService.Domain.Interfaces;

namespace FoodService.Application.UseCases.QueryHandlers.DayResult
{
    public class GetDayResultsByPeriodQueryHandler 
        : IQueryHandler<GetDayResultsByPeriodQuery, IEnumerable<DayResultResponse>?>
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

        public async Task<IEnumerable<DayResultResponse>?> Handle(
            GetDayResultsByPeriodQuery request,
            CancellationToken cancellationToken)
        {
            await _userService.CheckProfileBelongingAsync(
                request.UserId,
                request.ProfileId);

            var dayResults = await _unitOfWork.DayResultRepository.GetAllByPeriodAsync(
                request.ProfileId, 
                request.StartDate, 
                request.EndDate);

            var dayResultsDTO = _mapper.Map<List<DayResultResponse>>(dayResults);

            return dayResultsDTO;
        }
    }
}
