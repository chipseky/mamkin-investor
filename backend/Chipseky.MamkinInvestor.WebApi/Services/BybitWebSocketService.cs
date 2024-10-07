using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Chipseky.MamkinInvestor.Infrastructure;
using Chipseky.MamkinInvestor.Infrastructure.Options;
using Chipseky.MamkinInvestor.WebApi.Jobs;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Options;

namespace Chipseky.MamkinInvestor.WebApi.Services;

public class BybitWebSocketService
{
    private readonly InfluxDbService _influxDbService;
    private readonly BybitSettings _bybitSettings;
    private readonly ILogger<WebSocketsBackgroundService> _logger;
    private readonly ConcurrentQueue<byte[]> _messageQueue = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly CancellationTokenSource _ctsQueueHandler = new();

    public BybitWebSocketService(
        InfluxDbService influxDbService,
        IOptions<BybitSettings> options,
        ILogger<WebSocketsBackgroundService> logger)
    {
        _influxDbService = influxDbService;
        _bybitSettings = options.Value;
        _logger = logger;
    }

    public async Task Start(IEnumerable<string> tickers, CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                ResetCancellationTokenSources();

                // ReSharper disable once PossibleMultipleEnumeration
                await StartWebSocketClient(tickers, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected WebSocketsBackgroundService error!");
            }

            await Task.Delay(3000, stoppingToken);
        }
        
        // ReSharper disable once FunctionNeverReturns
    }
    
    private void ResetCancellationTokenSources()
    {
        _cts.TryReset();
        _ctsQueueHandler.TryReset();
    }

    private async Task StartWebSocketClient(IEnumerable<string> tickers, CancellationToken stoppingToken)
    {
        using var wsClient = new ClientWebSocket();
        
        await wsClient.ConnectAsync(new Uri(_bybitSettings.WsSpotUrl), stoppingToken);
        
        _logger.LogInformation($"connected: {_bybitSettings.WsSpotUrl}");
         
        await wsClient.SendAsync(GetSubscribeMessageBytes(tickers), WebSocketMessageType.Text, true, stoppingToken);
        
        _logger.LogInformation("subscription has been successfully sent.");

        _ = Task.Run(StartQueueHandler, stoppingToken);

        var buffer = new byte[1024 * 4];
        try
        {
            while (wsClient.State == WebSocketState.Open && !_ctsQueueHandler.Token.IsCancellationRequested)
            {
                var result = await wsClient.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.LogInformation("connection has been closed by server.");
                    await wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "connection has been closed by server.",
                        CancellationToken.None);
                }
                else
                {
                    // copy data and enqueue
                    var messageBytes = new byte[result.Count];
                    Array.Copy(buffer, messageBytes, result.Count);
                    _messageQueue.Enqueue(messageBytes);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebSocket handling exception!");
        }
        finally
        {
            await _cts.CancelAsync();
        }
    }

    private ArraySegment<byte> GetSubscribeMessageBytes(IEnumerable<string> tickers)
    {
        tickers = tickers.Select(t => $"tickers.{t}");

        var message = $$"""
                        {
                          "op": "subscribe",
                          "args": ["{{string.Join(@""", """, tickers)}}"]
                        }
                        """;
        
        var subscribeMessageBytes = Encoding.UTF8.GetBytes(message);

        return new ArraySegment<byte>(subscribeMessageBytes);
    }
    
    private async Task StartQueueHandler()
    {
        try
        {
            while (!_cts.Token.IsCancellationRequested)
                await HandleMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handling message error!");
        }
        finally
        {
            await _ctsQueueHandler.CancelAsync();
        }
    }
    
    private async Task HandleMessage()
    {
        byte[] messageBytes;
        if (_messageQueue.TryDequeue(out messageBytes!))
        {
            try
            {
                (string ticker, long ts, decimal lastPrice) = ParseMessageBytes(messageBytes);

                var point = PointData.Measurement("tickers")
                    .Tag("ticker", ticker)
                    .Field("value", lastPrice)
                    .Timestamp(ts, WritePrecision.Ms);

                await _influxDbService.Write("bybit-bucket", point);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"parse websocket message error: {Encoding.UTF8.GetString(messageBytes)}");
            }
        }
        else
        {
            // Если в очереди нет сообщений, ждем небольшое время
            await Task.Delay(10);
        }
    }
    
    private static (string ticker, long timestamp, decimal lastPrice) ParseMessageBytes(byte[] messageBytes)
    {
        using var doc = JsonDocument.Parse(messageBytes);
        var root = doc.RootElement;
        var ts = root.GetProperty("ts").GetInt64();
        var lastPrice = decimal.Parse(root.GetProperty("data").GetProperty("lastPrice").GetString()!);
        var ticker = root.GetProperty("data").GetProperty("symbol").GetString()!;

        return (ticker, ts, lastPrice);
    }
}