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

        public async Task AddListenerAsync(Func<BasicDeliverEventArgs, Task> handler, CancellationToken cancellationToken)
        {
            await CreateQueueIfNotExistsAsync(_queueName, cancellationToken);

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

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: _consumer, cancellationToken);
        }
    }
}
