using Chipseky.MamkinInvestor.WebApi.Extensions;
using Chipseky.MamkinInvestor.WebApi.Options;
using Chipseky.MamkinInvestor.WebApi.Services;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Chipseky.MamkinInvestor.WebApi.Jobs;

public class TelegramBotBackgroundService : BackgroundService
{
    private readonly string _tgBotAccessToken;
    private readonly long _chatId;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<TelegramBotBackgroundService> _logger;

    public TelegramBotBackgroundService(IServiceScopeFactory serviceScopeFactory, IOptions<TelegramSettings> options, ILogger<TelegramBotBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _tgBotAccessToken = options.Value.BotAccessToken;
        _chatId = options.Value.GroupChatId;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                await StartReceiving(stoppingToken);
            
                await Task.Delay(Timeout.Infinite, stoppingToken); // https://stackoverflow.com/a/73735099
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in TelegramBotBackgroundService bleat'");
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private async Task StartReceiving(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var botClient = GetBotClient(scope);
            
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [ UpdateType.Message ],
            ThrowPendingUpdates = true,
        };
        
        botClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cancellationToken); // Запускаем бота
        
        var mamkinInvestorBot = await botClient.GetMeAsync(cancellationToken); // Создаем переменную, в которую помещаем информацию о нашем боте.
        _logger.LogInformation($"Бот {mamkinInvestorBot.FirstName} запущен!");
    }
    
    private TelegramBotClient GetBotClient(IServiceScope scope)
    {
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient("tg_bot_pooling_client");
        return new TelegramBotClient(_tgBotAccessToken, httpClient);
    }
    
    private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    var message = update.Message!;
                    var user = message.From!;

                    _logger.LogInformation($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                    using var scope = _serviceScopeFactory.CreateScope();
                    
                    var hotCoinsService =
                        scope.ServiceProvider.GetRequiredService<HotCoinsService>();

                    var top10TradingPairs = await hotCoinsService.GetTop10TradingPairs();
                    
                    var messageText = message.Text is "делай" or "покажи"
                        ? top10TradingPairs.GetAsString()
                        : "мы вас усшылали";
                    
                    await botClient.SendTextMessageAsync(
                        _chatId,
                        "показал: \n\n" + messageText,
                        cancellationToken: cancellationToken);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TelegramBotBackgroundService.UpdateHandler");
        }
    }

    private Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var errorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}\n{apiRequestException.StackTrace}",
            _ => error.ToString()
        };

        _logger.LogError(errorMessage);
        
        return Task.CompletedTask;
    }
}