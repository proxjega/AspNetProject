using AspNetProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetProject.Services;

public class DraftCleanerService : BackgroundService
{
    private readonly ILogger<DraftCleanerService> _logger;
    private readonly IServiceScopeFactory _serviceFactory;

    public DraftCleanerService(ILogger<DraftCleanerService> logger, IServiceScopeFactory serviceFactory)
    {
        _logger = logger;
        _serviceFactory = serviceFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DraftCleanerService starting...");

        await DoWork(stoppingToken);

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(30));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWork(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("DraftCleanerService is stopping.");
        }
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {

        using var scope = _serviceFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        var cutoff = DateTime.UtcNow.AddDays(-1);

        int deleted = await db.Posts
            .Where(x => x.IsDraft && x.CreatedAt < cutoff)
            .ExecuteDeleteAsync(stoppingToken);

        _logger.LogInformation("DraftCleanerService cleaned {DeletedCount} Posts...", deleted);
    }
}