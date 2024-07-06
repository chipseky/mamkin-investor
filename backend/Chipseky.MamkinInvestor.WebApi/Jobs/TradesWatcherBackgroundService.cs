using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.Domain.Repositories;

namespace Chipseky.MamkinInvestor.WebApi.Jobs;

public class TradesWatcherBackgroundService : BackgroundService
{
    private readonly ILogger<TradesWatcherBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TradesWatcherBackgroundService(
        ILogger<TradesWatcherBackgroundService> logger, 
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var realAdviser = scope.ServiceProvider.GetService<IRealAdviser>()!;
        var tradesRepository = scope.ServiceProvider.GetService<ITradesRepository>()!;
        var ordersManager = scope.ServiceProvider.GetService<OrdersManager>()!;

        while (true)
        {
            try
            {
                var openTrades = await tradesRepository.GetOpenTrades(stoppingToken);
                foreach (var openTrade in openTrades)
                    if (await realAdviser.ShouldSell(openTrade.Symbol))
                        await ordersManager.CreateSellOrder(openTrade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TradesWatcherBackgroundService.");
            }

            await Task.Delay(3000, stoppingToken);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}