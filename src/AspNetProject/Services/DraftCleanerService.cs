using AspNetProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetProject.Services;

public class DraftCleanerService
{
    private readonly ILogger<DraftCleanerService> _logger;
    private readonly ApplicationContext _db;

    public DraftCleanerService(ILogger<DraftCleanerService> logger, ApplicationContext db)
    {
        _logger = logger;
        _db = db;
    }

    // protected async Task ExecuteAsync(CancellationToken stoppingToken)
    // {
    //     _logger.LogInformation("DraftCleanerService starting...");

    //     await DoWork(stoppingToken);

    //     using PeriodicTimer timer = new(TimeSpan.FromSeconds(30));

    //     try
    //     {
    //         while (await timer.WaitForNextTickAsync(stoppingToken))
    //         {
    //             await DoWork(stoppingToken);
    //         }
    //     }
    //     catch (OperationCanceledException)
    //     {
    //         _logger.LogInformation("DraftCleanerService is stopping.");
    //     }
    // }

    public async Task Execute(CancellationToken stoppingToken)
    {

        var cutoff = DateTime.UtcNow.AddDays(-1);

        int deleted = await _db.Posts
            .Where(x => x.IsDraft && x.CreatedAt < cutoff)
            .ExecuteDeleteAsync(stoppingToken);

        _logger.LogInformation("DraftCleanerService cleaned {DeletedCount} Posts...", deleted);
    }
}