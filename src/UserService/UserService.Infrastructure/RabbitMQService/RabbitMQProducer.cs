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
        QueueName? queueName,
        ExchangeName? exchangeName,
        bool mandatory = true,
        CancellationToken cancellationToken = default)
    {
        var properties = new BasicProperties
        {
            Persistent = true
        };

        if (exchangeName != null)
        {
            await CreateExchangeIfNotExistsAsync(exchangeName.Value);
        }
        else if (queueName != null)
        {
            await CreateQueueIfNotExistsAsync(queueName.Value.ToString(), cancellationToken);
        }

        var body = Encoding.UTF8.GetBytes(message);

        try
        {
            await _channel.BasicPublishAsync(
                exchange: exchangeName?.ToString() ?? "",
                routingKey: queueName?.ToString() ?? "",
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
