namespace Chipseky.MamkinInvestor.WebApi.Jobs;

public class TrendsDetectorBackgroundService : BackgroundService
{
    private readonly ILogger<TrendsDetectorBackgroundService> _logger;
    private readonly string[] _symbols = ["TON/USDT"];

    public TrendsDetectorBackgroundService(ILogger<TrendsDetectorBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                await FindTrends();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in TradingBackgroundService bleat'");
            }

            await Task.Delay(10 * 1000, stoppingToken);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private async Task FindTrends()
    {
        foreach (var symbol in _symbols)
        {
            var isTrendFound = await IsUpTrend(symbol);
            if (isTrendFound)
            {
                //todo: notify by tg
            }
        }
    }

    private async Task<bool> IsUpTrend(string symbol)
    {
        var rnd = new Random();
        return rnd.Next(50) > 25;
    }
}