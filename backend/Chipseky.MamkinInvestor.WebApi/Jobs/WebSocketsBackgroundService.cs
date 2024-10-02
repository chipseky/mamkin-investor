using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Chipseky.MamkinInvestor.Infrastructure;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace Chipseky.MamkinInvestor.WebApi.Jobs;

public class WebSocketsBackgroundService : BackgroundService
{
    private const string WsUrl = "wss://stream.bybit.com/v5/public/spot";
    private readonly IEnumerable<string> _tickers = new[] { "BTCUSDT", "ETHUSDT" }.Select(t => $"tickers.{t}");
    
    private readonly InfluxService _influxService;
    private readonly ILogger<WebSocketsBackgroundService> _logger;
    private readonly ConcurrentQueue<byte[]> _messageQueue = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly CancellationTokenSource _ctsQueueHandler = new();

    public WebSocketsBackgroundService(InfluxService influxService, ILogger<WebSocketsBackgroundService> logger)
    {
        _influxService = influxService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                ResetCancellationTokenSources();

                await StartWebSocketClient(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Unexpected WebSocketsBackgroundService error!");
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

    private async Task StartWebSocketClient(CancellationToken stoppingToken)
    {
        using var wsClient = new ClientWebSocket();
        
        await wsClient.ConnectAsync(new Uri(WsUrl), stoppingToken);
        
        _logger.LogInformation($"connected: {WsUrl}");
         
        await wsClient.SendAsync(GetSubscribeMessageBytes(), WebSocketMessageType.Text, true, stoppingToken);
        
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

    private ArraySegment<byte> GetSubscribeMessageBytes()
    {
        var message = $$"""
                        {
                          "op": "subscribe",
                          "args": ["{{string.Join(@""", """, _tickers)}}"]
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

                await _influxService.Write(point);
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