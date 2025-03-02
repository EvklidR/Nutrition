using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UserService.Infrastructure.RabbitMQService.Settings;

namespace UserService.Infrastructure.RabbitMQService
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

        protected async Task CreateQueueIfNotExistsAsync(string queueName, Dictionary<string, object?>? args = null)
        {
            await _channel.QueueDeclareAsync(queueName, autoDelete: false, exclusive: false, durable: true, arguments: args);
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
