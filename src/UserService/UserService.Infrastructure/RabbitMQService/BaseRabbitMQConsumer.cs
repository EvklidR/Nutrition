using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using UserService.Infrastructure.RabbitMQService.Settings;

namespace UserService.Infrastructure.RabbitMQService
{
    public class BaseRabbitMQConsumer : BaseRabbitMQService
    {
        protected readonly AsyncEventingBasicConsumer _consumer;
        protected string _queueName;

        public BaseRabbitMQConsumer(IOptions<RabbitMqSettings> options) : base(options)
        {
            _consumer = new AsyncEventingBasicConsumer(_channel);
        }

        public async Task AddListenerAsync(Func<BasicDeliverEventArgs, Task> handler)
        {
            await CreateQueueIfNotExistsAsync(_queueName);

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

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: _consumer);
        }
    }
}
