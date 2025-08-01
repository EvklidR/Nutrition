using AutoMapper;
using FoodService.Application.Exceptions;
using FoodService.Domain.Interfaces;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Commands.DayResult;
using Newtonsoft.Json;
using FoodService.Application.Enums;

namespace FoodService.Application.UseCases.CommandHandlers.DayResult
{
    public class UpdateDayResultHandler : ICommandHandler<UpdateDayResultCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IBrokerService _brokerService;

        public UpdateDayResultHandler(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IUserService userService, 
            IBrokerService brokerService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
            _brokerService = brokerService;
        }

        public async Task Handle(UpdateDayResultCommand request, CancellationToken cancellationToken)
        {
            var dayResult = await _unitOfWork.DayResultRepository.GetByIdAsync(request.UpdateDayResultDTO.Id);

            if (dayResult == null)
            {
                throw new NotFound("DayResult not found");
            }

            await _userService.CheckProfileBelongingAsync(request.UserId, dayResult.ProfileId);

            if (dayResult.Date == DateOnly.FromDateTime(DateTime.Now) && 
                request.UpdateDayResultDTO.Weight != dayResult.Weight)
            {
                var message = JsonConvert.SerializeObject(new
                {
                    ProfileId = dayResult.ProfileId,
                    NewWeight = request.UpdateDayResultDTO.Weight
                });

                await _brokerService.PublishMessageAsync(message, QueueName.DayResultWeightChanged);
            }

            _mapper.Map(request.UpdateDayResultDTO, dayResult);

            _unitOfWork.DayResultRepository.Update(dayResult);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
