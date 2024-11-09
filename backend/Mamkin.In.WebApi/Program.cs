using Mamkin.In.Domain;
using Mamkin.In.Domain.Repositories;
using Mamkin.In.Infrastructure;
using Mamkin.In.Infrastructure.ApiClients;
using Mamkin.In.Infrastructure.DbRepositories;
using Mamkin.In.Infrastructure.Ef;
using Mamkin.In.Infrastructure.Influx;
using Mamkin.In.Infrastructure.Options;
using Mamkin.In.Infrastructure.ReplicationSlots;
using Mamkin.In.WebApi.Extensions;
using Mamkin.In.WebApi.Infrastructure.Database;
using Mamkin.In.WebApi.Infrastructure.ReplicationSlots;
using Mamkin.In.WebApi.Jobs;
using Mamkin.In.WebApi.Options;
using Mamkin.In.WebApi.QueryHandlers;
using Mamkin.In.WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadDotEnv();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["ConnectionStrings:Redis"];
});

builder.Services.AddHttpClient("tg_bot_client")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(2)
    })
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan); // Disable rotation, as it is handled by PooledConnectionLifetime

builder.Services.AddHttpClient("tg_bot_pooling_client")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(2)
    })
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan); // Disable rotation, as it is handled by PooledConnectionLifetime

builder.Services.AddHttpClient("bybit_client")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(2)
    })
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan); // Disable rotation, as it is handled by PooledConnectionLifetime

builder.Services.AddHttpClient<IForecastApi, ForecastApiClient>("forecast_client", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ConnectionStrings:Forecast"]!);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddPolicyHandler((sp, _) => RetryPoliceHelper.GetRetryPolicy(sp.GetService<ILoggerFactory>()!));


var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("MamkinInvestorDatabase"));
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseNpgsql(dataSource)
        .UseSnakeCaseNamingConvention());

builder.Services.Configure<TelegramSettings>(builder.Configuration.GetSection("Telegram"));
builder.Services.Configure<BybitSettings>(builder.Configuration.GetSection("Bybit"));
builder.Services.Configure<TradingBackgroundServiceSettings>(builder.Configuration.GetSection("TradingBackgroundServiceSettings"));

builder.Services.Configure<InfluxDbSettings>(builder.Configuration.GetSection("InfluxDb"));
builder.Services.AddSingleton<InfluxDbService>();

builder.Services.AddScoped<MarketDataService>();
// builder.Services.AddScoped<IOrdersApi, BybitOrdersApi>();
builder.Services.AddScoped<IOrdersApi, FakeOrdersApi>();
builder.Services.AddScoped<BybitOrdersApiClient>();
builder.Services.AddScoped<BybitHistoryApiClient>();
builder.Services.AddScoped<OrdersManager>();
builder.Services.AddScoped<IRealAdviser, RealAdviser>();
builder.Services.AddScoped<IForecastsRepository, ForecastsRepository>();
builder.Services.AddScoped<IPredefinedSymbolsRepository, PredefinedSymbolsRepository>();
builder.Services.AddScoped<IMarketDataProvider, MarketDataProvider>();
builder.Services.AddScoped<ITradeEventsRepository, TradeEventsRepository>();
builder.Services.AddScoped<Trader>();
builder.Services.AddScoped<BybitWebSocketService>();
builder.Services.AddHostedService<TelegramBotBackgroundService>();
builder.Services.AddHostedService<TradingBackgroundService>();
builder.Services.AddHostedService<TradesWatcherBackgroundService>();
builder.Services.AddHostedService<TrendsDetectorBackgroundService>();
builder.Services.AddHostedService<WebSocketsBackgroundService>();

builder.Services.Configure<PostgresReplicationServiceOptions>(o =>
{
    o.ConnectionString = builder.Configuration.GetConnectionString("MamkinInvestorDatabase")!;
    o.PublicationName = "mamkin_investor_trade_events_pub";
    o.ReplicationSlotName = "mamkin_investor_trade_events_slot";
});
builder.Services.AddScoped<PostgresReplicationService>();
builder.Services.AddHostedService<PostgresReplicationListener>();

builder.Services.AddScoped<TradesQueryHandler>();
builder.Services.AddScoped<TradeEventsQueryHandler>();

builder.Services.AddScoped<ITradesRepository, TradesRepository>();
builder.Services.AddScoped<TradeEventsHandler>();

builder.Services.AddScoped<LotSizeFilterService>();

var app = builder.Build();

app.MigrateDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(b => b
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins(builder.Configuration.GetSection("CORS:Origins").Get<string[]>()!)
    .SetIsOriginAllowedToAllowWildcardSubdomains()
    .WithExposedHeaders("Content-Disposition", "Content-Length")
    .SetPreflightMaxAge(TimeSpan.FromMinutes(15)));

app.UseHttpsRedirection();

app.MapControllers();

app.Run();