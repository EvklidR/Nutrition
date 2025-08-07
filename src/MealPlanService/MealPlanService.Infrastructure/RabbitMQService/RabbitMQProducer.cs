using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;
using MealPlanService.Infrastructure.Enums;
using MealPlanService.Infrastructure.RabbitMQService.Settings;

namespace MealPlanService.Infrastructure.RabbitMQService
{
    internal class RabbitMQProducer : BaseRabbitMQService, IBrokerService
    {
        public RabbitMQProducer(IOptions<RabbitMqSettings> options) : base(options) { }

        private static readonly AsyncRetryPolicy _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

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
                await CreateQueueIfNotExistsAsync(queueName.Value);
            }

            var body = Encoding.UTF8.GetBytes(message);

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _channel.BasicPublishAsync(
                    exchange: exchangeName.ToString() ?? "",
                    routingKey: queueName.ToString() ?? "",
                    mandatory: mandatory,
                    basicProperties: properties,
                    body: body);
            });
        }
    }
}
