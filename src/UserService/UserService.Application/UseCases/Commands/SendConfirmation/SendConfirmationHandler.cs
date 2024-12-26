using EventsService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.UseCases.Commands
{
    public class SendConfirmationHandler : ICommandHandler<SendConfirmationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailSender;

        public SendConfirmationHandler(IUnitOfWork unitOfWork, IEmailService emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public async Task Handle(SendConfirmationCommand request, CancellationToken cancellationToken)
        {
            var code = _emailSender.GenerateCode();

            var newConfirmation = new Confirmation() {
                Email = request.email,
                Code =  code
            };

            _emailSender.SendEmail(request.email, "Confirmation", $"It is your confirmation code {code}");

            _unitOfWork.ConfirmationRepository.Add(newConfirmation);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
