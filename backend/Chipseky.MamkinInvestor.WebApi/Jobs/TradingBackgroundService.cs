using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Extensions;
using Chipseky.MamkinInvestor.WebApi.Options;
using Chipseky.MamkinInvestor.WebApi.Services;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Chipseky.MamkinInvestor.WebApi.Jobs;

public class TradingBackgroundService : BackgroundService
{
    private readonly string _tgBotAccessToken;
    private readonly long _chatId;
    private readonly int _checkPeriodInSeconds;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<TradingBackgroundService> _logger;

    public TradingBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<TelegramSettings> telegramSettingsOptions,
        IOptions<TradingBackgroundServiceSettings> tradingBackgroundServiceSettingsOptions,
        ILogger<TradingBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _tgBotAccessToken = telegramSettingsOptions.Value.BotAccessToken;
        _chatId = telegramSettingsOptions.Value.GroupChatId;
        _checkPeriodInSeconds = tradingBackgroundServiceSettingsOptions.Value.CheckPeriodInSeconds;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                
                var trader = scope.ServiceProvider.GetRequiredService<Trader>();
                var marketDataService = scope.ServiceProvider.GetRequiredService<MarketDataService>();

                var symbolsDetails = await marketDataService.GetSymbolsDetails();

                await trader.Feed(symbolsDetails);

                var botClient = GetBotClient(scope);

                await botClient.SendTextMessageAsync(
                    _chatId,
                    symbolsDetails.GetAsString(),
                    cancellationToken: stoppingToken
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in TradingBackgroundService bleat'");
            }

            await Task.Delay(_checkPeriodInSeconds * 1000, stoppingToken);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private TelegramBotClient GetBotClient(IServiceScope scope)
    {
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient("tg_bot_client");
        return new TelegramBotClient(_tgBotAccessToken, httpClient);
    }
}