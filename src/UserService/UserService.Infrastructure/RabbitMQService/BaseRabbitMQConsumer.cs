using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using UserService.Infrastructure.RabbitMQService.Settings;
using UserService.Contracts.Broker.Enums;

namespace UserService.Infrastructure.RabbitMQService;

public class BaseRabbitMQConsumer : BaseRabbitMQService
{
    protected readonly AsyncEventingBasicConsumer _consumer;
    protected QueueName _queueName;

    public BaseRabbitMQConsumer(IOptions<RabbitMqSettings> options) : base(options)
    {
        _consumer = new AsyncEventingBasicConsumer(_channel);
    }

    public async Task AddListenerAsync(
        Func<BasicDeliverEventArgs, Task> handler,
        ExchangeName? exchangeName = null,
        string exchangeType = ExchangeType.Fanout,
        string routingKey = "",
        CancellationToken cancellationToken = default)
    {
        if (exchangeName.HasValue)
        {
            await CreateExchangeIfNotExistsAsync(exchangeName.Value);

            await CreateQueueIfNotExistsAsync(_queueName.ToString(), cancellationToken);

            await BindQueueToExchangeAsync(exchangeName.Value, _queueName);
        }
        else
        {
            await CreateQueueIfNotExistsAsync(_queueName.ToString(), cancellationToken);
        }

        _consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                await handler(ea);

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} [ERROR] Message processing failed: {ex.Message}");

                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken);
            }
        };

        await _channel.BasicConsumeAsync(queue: _queueName.ToString(), autoAck: false, consumer: _consumer, cancellationToken);
    }
}
