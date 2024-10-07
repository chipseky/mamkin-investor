using Chipseky.MamkinInvestor.WebApi.Services;

namespace Chipseky.MamkinInvestor.WebApi.Jobs;

public class WebSocketsBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public WebSocketsBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var webSocketService = scope.ServiceProvider.GetService<BybitWebSocketService>()!;
        
        await Task.WhenAll(
            webSocketService.Start(["BTCUSDT"], stoppingToken),
            webSocketService.Start(["ETHUSDT"], stoppingToken));
    }
}