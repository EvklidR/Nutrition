using FoodService.Application.Exceptions;
using FoodService.Domain.Interfaces;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.DayResult;
using AutoMapper;
using FoodService.Application.UseCases.Queries.DayResult;

namespace FoodService.Application.UseCases.QueryHandlers.DayResult
{
    public class GetDayResultsByPeriodQueryHandler 
        : IQueryHandler<GetDayResultsByPeriodQuery, IEnumerable<DayResultDTO>?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICheckUserService _userService;
        private readonly IMapper _mapper;

        public GetDayResultsByPeriodQueryHandler(IUnitOfWork unitOfWork, ICheckUserService userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DayResultDTO>?> Handle(
            GetDayResultsByPeriodQuery request,
            CancellationToken cancellationToken)
        {
            var isProfileBelongUser = await _userService.CheckProfileBelonging(
                request.UserId,
                request.ProfileId);

            if (!isProfileBelongUser)
            {
                throw new Forbidden("You dont have access to this day result");
            }

            var dayResults = await _unitOfWork.DayResultRepository.GetAllByPeriodAsync(
                request.ProfileId, 
                request.StartDate, 
                request.EndDate);

            var dayResultsDTO = _mapper.Map<List<DayResultDTO>>(dayResults);

            return dayResultsDTO;
        }
    }
}
