using DevTrack.Service.Interfaces;

namespace DevTrack.Api.BackgroundServices;

public class ReminderGeneratorBackgroundService : BackgroundService
{
    private static readonly TimeSpan DailyRunTimeUtc = TimeSpan.FromHours(3);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReminderGeneratorBackgroundService> _logger;

    public ReminderGeneratorBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ReminderGeneratorBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReminderGeneratorBackgroundService starting; will run daily at {Hour:00}:00 UTC.", DailyRunTimeUtc.Hours);

        try
        {
            await RunOnceSafelyAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = TimeUntilNextRun(DateTime.UtcNow);
                _logger.LogDebug("Next reminder generator run in {Delay}.", delay);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException) { return; }

                await RunOnceSafelyAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Service stopping; ignore.
        }
    }

    private async Task RunOnceSafelyAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var generator = scope.ServiceProvider.GetRequiredService<IReminderGenerator>();
            var count = await generator.GenerateForAllUsersAsync(ct);
            _logger.LogInformation("Scheduled reminder generator run completed. Created {Count} reminders.", count);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scheduled reminder generator run failed; will retry at next scheduled time.");
        }
    }

    internal static TimeSpan TimeUntilNextRun(DateTime utcNow)
    {
        var todayRun = utcNow.Date.Add(DailyRunTimeUtc);
        var nextRun = utcNow < todayRun ? todayRun : todayRun.AddDays(1);
        return nextRun - utcNow;
    }
}
