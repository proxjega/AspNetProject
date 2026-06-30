using RabbitMQ.Client;

namespace AspNetProject.Services;

public interface IRabbitMqConnection : IAsyncDisposable
{
    Task<IChannel> CreateChannelAsync();
}

public class RabbitMqConnection : IRabbitMqConnection
{
    private readonly Task<IConnection> _connectionTask;

    public RabbitMqConnection(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            VirtualHost = "/",
            UserName = "guest",
            Password = "guest",
            Port = int.Parse(configuration["RabbitMq:Port"] ?? "5672"),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        // Start connecting immediately; reused by all callers.
        _connectionTask = factory.CreateConnectionAsync();
    }

    public async Task<IChannel> CreateChannelAsync()
    {
        var connection = await _connectionTask;
        return await connection.CreateChannelAsync();
    }

    public async ValueTask DisposeAsync()
    {
        var connection = await _connectionTask;
        await connection.CloseAsync();
        connection.Dispose();
    }
}