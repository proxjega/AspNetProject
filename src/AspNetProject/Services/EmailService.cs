using System.Text;
using System.Text.Json;
using AspNetProject.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AspNetProject.Services;

public class EmailService : IHostedService
{
    private readonly IRabbitMqConnection _connection;
    private readonly ILogger<EmailService> _logger;
    private IChannel? _channel;

    public EmailService(IRabbitMqConnection connection, ILogger<EmailService> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync("app.events", ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);
        var queueName = (await _channel.QueueDeclareAsync(
            queue: "email-service.user-created",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken)).QueueName;

        await _channel.QueueBindAsync(queueName, "app.events", routingKey: "user.created", cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            var json = Encoding.UTF8.GetString(ea.Body.Span);
            var evt = JsonSerializer.Deserialize<UserCreatedEvent>(json);

            // send email here
            _logger.LogInformation("Sending welcome email to {Email}", evt?.Email);

            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process UserCreated event");
            await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
        }
    }
}