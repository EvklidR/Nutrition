using AutoMapper;
using MediatR;
using FoodService.Application.Exceptions;
using FoodService.Domain.Interfaces;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.DayResult;
using FoodService.Application.UseCases.Commands.DayResult;

namespace FoodService.Application.UseCases.CommandHandlers.DayResult
{
    public class CreateDayResultCommandHandler 
        : ICommandHandler<CreateDayResultCommand, DayResultDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CreateDayResultCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<DayResultDTO> Handle(
            CreateDayResultCommand request,
            CancellationToken cancellationToken)
        {
            var doesProfileBelongUser = await _userService.CheckProfileBelonging(
                request.UserId, 
                request.CreateDayResultDTO.ProfileId);

            if (!doesProfileBelongUser)
            {
                throw new Forbidden("You dont have access to this meal");
            }

            var dayResult = _mapper.Map<Domain.Entities.DayResult>(request.CreateDayResultDTO);

            _unitOfWork.DayResultRepository.Add(dayResult);

            await _unitOfWork.SaveChangesAsync();

            var dayResultDTO = _mapper.Map<DayResultDTO>(dayResult);

            return dayResultDTO;
        }
    }
}
