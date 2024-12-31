using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Mamkin.In.Infrastructure.Options;
using Mamkin.In.WebApi.Jobs;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Mamkin.In.Infrastructure.Influx;
using Microsoft.Extensions.Options;

namespace Mamkin.In.WebApi.Services;

public class BybitWebSocketService
{
    private readonly InfluxDbService _influxDbService;
    private readonly BybitSettings _bybitSettings;
    private readonly ILogger<WebSocketsBackgroundService> _logger;
    private readonly ConcurrentQueue<byte[]> _messageQueue = new();
    private CancellationTokenSource _cts;

    public BybitWebSocketService(
        InfluxDbService influxDbService,
        IOptions<BybitSettings> options,
        ILogger<WebSocketsBackgroundService> logger)
    {
        _influxDbService = influxDbService;
        _bybitSettings = options.Value;
        _logger = logger;
    }

    public async Task Start(IEnumerable<string> tickers, CancellationToken cancellationToken)
    {
        while (true)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                // ReSharper disable once PossibleMultipleEnumeration
                await StartWebSocketClient(tickers, _cts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected WebSocketsBackgroundService error!");
            }
            finally
            {
                _cts.Dispose();
            }

            await Task.Delay(3000, cancellationToken);
        }
        
        // ReSharper disable once FunctionNeverReturns
    }
    
    private async Task StartWebSocketClient(IEnumerable<string> tickers, CancellationToken cancellationToken)
    {
        using var wsClient = new ClientWebSocket();
        
        await wsClient.ConnectAsync(new Uri(_bybitSettings.WsSpotUrl), cancellationToken);
        
        _logger.LogInformation($"connected: {_bybitSettings.WsSpotUrl}");
         
        await wsClient.SendAsync(GetSubscribeMessageBytes(tickers), WebSocketMessageType.Text, true, cancellationToken);
        
        _logger.LogInformation("subscription has been successfully sent.");

        
        _ = Task.Run(StartQueueHandler, cancellationToken);

        var buffer = new byte[1024 * 4];
        try
        {
            while (wsClient.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await wsClient.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

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
        while (!_cts.Token.IsCancellationRequested)
        {
            if (_messageQueue.TryDequeue(out var messageBytes))
            {
                try
                {
                    await HandleMessage(messageBytes);
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
    }

    private async Task HandleMessage(byte[] messageBytes)
    {
        (string ticker, long ts, decimal lastPrice) = ParseMessageBytes(messageBytes);

        var point = PointData.Measurement("tickers")
            .Tag("ticker", ticker)
            .Field("value", lastPrice)
            .Timestamp(ts, WritePrecision.Ms);

        await _influxDbService.Write("bybit-bucket", point);
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