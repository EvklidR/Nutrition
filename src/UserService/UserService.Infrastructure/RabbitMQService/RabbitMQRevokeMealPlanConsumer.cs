using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using UserService.Infrastructure.RabbitMQService.Settings;

namespace UserService.Infrastructure.RabbitMQService
{
    public class RabbitMQRevokeMealPlanConsumer : BaseRabbitMQService
    {
        private readonly AsyncEventingBasicConsumer _consumer;

        public RabbitMQRevokeMealPlanConsumer(IOptions<RabbitMqSettings> options) : base(options)
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} [ERROR] Message processing failed: {ex.Message}");
                }
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: _consumer);
        }
    }
}
