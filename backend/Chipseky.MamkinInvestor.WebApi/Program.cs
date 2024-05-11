using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Extensions;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;
using Chipseky.MamkinInvestor.WebApi.Jobs;
using Chipseky.MamkinInvestor.WebApi.Options;
using Chipseky.MamkinInvestor.WebApi.Queries;
using Chipseky.MamkinInvestor.WebApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadDotEnv();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MamkinInvestorDatabase")));

builder.Services.Configure<TelegramSettings>(builder.Configuration.GetSection("Telegram"));
builder.Services.Configure<BybitSettings>(builder.Configuration.GetSection("Bybit"));
builder.Services.Configure<TradingBackgroundServiceSettings>(builder.Configuration.GetSection("TradingBackgroundServiceSettings"));

builder.Services.AddSingleton<TradingPairsManager>();
builder.Services.AddScoped<HotDerivativesService>();
builder.Services.AddScoped<IOrdersApi, OrdersApi>();
builder.Services.AddScoped<OrdersManager>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<Trader>();
builder.Services.AddHostedService<TelegramBotBackgroundService>();
builder.Services.AddHostedService<TradingBackgroundService>();

builder.Services.AddScoped<OrdersTableDataQueryHandler>();

var app = builder.Build();

app.MigrateDatabase();

// Configure the HTTP request pipeline.
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