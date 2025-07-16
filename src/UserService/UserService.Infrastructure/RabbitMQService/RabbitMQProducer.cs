using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using UserService.Contracts.Broker;
using UserService.Contracts.Broker.Enums;
using UserService.Infrastructure.RabbitMQService.Settings;

namespace UserService.Infrastructure.RabbitMQService;

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

        await CreateQueueIfNotExistsAsync(queueName.ToString(), cancellationToken);

        var body = Encoding.UTF8.GetBytes(message);

        try
        {
            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: queueName.ToString(),
                mandatory: mandatory,
                basicProperties: properties,
                body: body,
                cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при публикации сообщения: {ex.Message}");
            throw;
        }
    }
}
