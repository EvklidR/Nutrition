using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using FoodService.Infrastructure.RabbitMQService.Settings;

namespace FoodService.Infrastructure.RabbitMQService
{
    public class RabbitMQConsumer : BaseRabbitMQService
    {
        private readonly AsyncEventingBasicConsumer _consumer;

        public RabbitMQConsumer(IOptions<RabbitMqSettings> options) : base(options)
        {
            _consumer = new AsyncEventingBasicConsumer(_channel);
        }

        public async Task AddListenerAsync(string queueName, Func<BasicDeliverEventArgs, Task> handler)
        {
            await CreateQueueIfNotExistsAsync(queueName);

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

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: _consumer);
        }
    }
}
