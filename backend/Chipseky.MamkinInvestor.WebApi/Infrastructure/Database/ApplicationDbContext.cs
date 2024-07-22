using Chipseky.MamkinInvestor.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<Trade> Trades { get; set; }
    public DbSet<DbTradeEvent> TradeEvents { get; set; }
    public DbSet<Forecast> Forecasts { get; set; }
    
    public DbSet<PredefinedSymbol> PredefinedSymbols { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DbTradeEvent>(builder =>
        {
            builder.ToTable("trade_events");

            builder.HasKey(te => te.DbTradeEventId);
            builder.Property(te => te.DbTradeEventId).UseIdentityAlwaysColumn();
            builder.Property(te => te.Data).HasColumnType("json");
        });
        
        modelBuilder.Entity<Trade>(builder =>
        {
            builder.ToTable("trades");

            builder.HasKey(t => t.TradeId);
            builder.Property(t => t.History).HasColumnType("json");
            builder.Property(t => t.State).HasConversion<string>();
        });
        
        modelBuilder.Entity<PredefinedSymbol>(builder =>
        {
            builder.ToTable("selected_symbols");

            builder.HasKey(ss => ss.Symbol);
            builder.Property(ss => ss.Symbol).HasMaxLength(32);
            // builder.Property(ss => ss.ForecastedSellOffset).HasConversion(new TimeSpanToTicksConverter());
        });
    }
}

public class DbTradeEvent
{
    // because I cannot insert entity without key: https://stackoverflow.com/a/75517604/6160271
    public long DbTradeEventId { get; set; }
    public string Type { get; set; }
    public object Data { get; set; }
}