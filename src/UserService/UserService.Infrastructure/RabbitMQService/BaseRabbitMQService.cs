using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UserService.Infrastructure.RabbitMQService.Settings;

namespace UserService.Infrastructure.RabbitMQService;

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

    protected async Task CreateQueueIfNotExistsAsync(string queueName, CancellationToken cancellationToken)
    {
        string dlqName = queueName + "-dlq";

        var mainQueueArgs = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", dlqName }
        };

        await _channel.QueueDeclareAsync(
            queueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false, 
            arguments: mainQueueArgs, 
            cancellationToken: cancellationToken);

        var dlqArgs = new Dictionary<string, object?>
        {
            { "x-message-ttl", 60000 },
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", queueName }
        };

        await _channel.QueueDeclareAsync(
            dlqName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false, 
            arguments: dlqArgs,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
