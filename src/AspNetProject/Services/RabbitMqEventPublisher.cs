using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace AspNetProject.Services;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, string routingKey, CancellationToken cancellationToken = default);
}

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IRabbitMqConnection _connection;
    private const string ExchangeName = "app.events";

    public RabbitMqEventPublisher(IRabbitMqConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string routingKey, CancellationToken cancellationToken = default)
    {
        await using var channel = await _connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);
    }
}