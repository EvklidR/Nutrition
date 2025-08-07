using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using MealPlanService.Infrastructure.RabbitMQService.Settings;
using MealPlanService.Infrastructure.Enums;

namespace MealPlanService.Infrastructure.RabbitMQService;

public class BaseRabbitMQConsumer : BaseRabbitMQService
{
    private readonly AsyncEventingBasicConsumer _consumer;
    protected QueueName _queueName;

    public BaseRabbitMQConsumer(IOptions<RabbitMqSettings> options) : base(options)
    {
        _consumer = new AsyncEventingBasicConsumer(_channel);
    }

    public async Task AddListenerAsync(
        Func<BasicDeliverEventArgs, Task> handler,
        ExchangeName? exchangeName = null,
        string exchangeType = ExchangeType.Fanout,
        string routingKey = "")
    {
        if (exchangeName.HasValue)
        {
            await CreateExchangeIfNotExistsAsync(exchangeName.Value);

            await CreateQueueIfNotExistsAsync(_queueName);

            await BindQueueToExchangeAsync(exchangeName.Value, _queueName);
        }
        else
        {
            await CreateQueueIfNotExistsAsync(_queueName);
        }

        _consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                await handler(ea);

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} [ERROR] Message processing failed: {ex.Message}");

                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await _channel.BasicConsumeAsync(queue: _queueName.ToString(), autoAck: false, consumer: _consumer);
    }
}
