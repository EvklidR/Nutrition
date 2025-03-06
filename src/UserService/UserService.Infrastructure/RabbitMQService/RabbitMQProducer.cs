using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using UserService.Application.Enums;
using UserService.Application.Interfaces;
using UserService.Infrastructure.RabbitMQService.Settings;

namespace UserService.Infrastructure.RabbitMQService
{
    internal class RabbitMQProducer : BaseRabbitMQService, IBrokerService
    {
        public RabbitMQProducer(IOptions<RabbitMqSettings> options) : base(options) { }

        public async Task PublishMessageAsync(
            string message,
            QueueName queueName,
            string exchange = "",
            bool mandatory = true,
            CancellationToken cancellationToken = default)
        {
            var properties = new BasicProperties
            {
                Persistent = true
            };

            await CreateQueueIfNotExistsAsync(queueName.ToString());

            var body = Encoding.UTF8.GetBytes(message);

            try
            {
                await _channel.BasicPublishAsync(
                    exchange: exchange,
                    routingKey: queueName.ToString(),
                    mandatory: mandatory,
                    basicProperties: properties,
                    body: body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при публикации сообщения: {ex.Message}");
                throw;
            }
        }
    }
}
