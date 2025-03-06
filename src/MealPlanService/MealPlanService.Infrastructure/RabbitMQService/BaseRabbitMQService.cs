using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using MealPlanService.Infrastructure.RabbitMQService.Settings;

namespace MealPlanService.Infrastructure.RabbitMQService
{
    public class BaseRabbitMQService : IAsyncDisposable
    {
        private readonly IConnection _connection;
        protected readonly IChannel _channel;

        public BaseRabbitMQService(IOptions<RabbitMqSettings> options)
        {
            var settings = options.Value;

            var factory = new ConnectionFactory
            {
                HostName = settings.Host,
                Port = settings.Port,
                UserName = settings.Username,
                Password = settings.Password,
                VirtualHost = settings.VirtualHost
            };

            var channelOpts = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true
            );

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync(channelOpts).GetAwaiter().GetResult();
        }

        protected async Task CreateQueueIfNotExistsAsync(string queueName)
        {
            string dlxName = queueName + "-dlx";
            string dlqName = queueName + "-dlq";

            var args = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", dlxName }
            };

            await _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, arguments: args);
            await _channel.ExchangeDeclareAsync(dlxName, ExchangeType.Direct);
            await _channel.QueueDeclareAsync(dlqName, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(dlqName, dlxName, queueName);
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
