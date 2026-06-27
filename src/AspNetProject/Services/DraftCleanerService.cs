namespace AspNetProject.Services;

public class DraftCleanerService : BackgroundService
{
    private readonly ILogger<DraftCleanerService> _logger;
    private int _executionCount;

    public DraftCleanerService(ILogger<DraftCleanerService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DraftCleanerService starting...");

        // When the timer should have no due-time, then do the work once now.
        await DoWork();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(30));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWork();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("DraftCleanerService is stopping.");
        }
    }

    private async Task DoWork()
    {
        int count = Interlocked.Increment(ref _executionCount);

        // Simulate work
        await Task.Delay(TimeSpan.FromSeconds(2));

        _logger.LogInformation("DraftCleanerService cleaning... Count: {Count}", count);
    }
}