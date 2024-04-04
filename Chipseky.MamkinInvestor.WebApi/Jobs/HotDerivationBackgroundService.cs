using Chipseky.MamkinInvestor.WebApi.Options;
using Chipseky.MamkinInvestor.WebApi.Services;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Chipseky.MamkinInvestor.WebApi.Jobs;

public class HotDerivationBackgroundService : BackgroundService
{
    private readonly string _tgBotAccessToken;
    private readonly long _chatId;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public HotDerivationBackgroundService(IServiceScopeFactory serviceScopeFactory, IOptions<TelegramSettings> options)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tgBotAccessToken = options.Value.BotAccessToken;
        _chatId = options.Value.GroupChatId;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var hotDerivationService =
                    scope.ServiceProvider.GetRequiredService<HotDerivationService>();
                
                var message = await hotDerivationService.GetAsString();

                var botClient = new TelegramBotClient(_tgBotAccessToken);
                
                await botClient.SendTextMessageAsync(
                    _chatId,
                    message,
                    cancellationToken: stoppingToken
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await Task.Delay(300000, stoppingToken);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}