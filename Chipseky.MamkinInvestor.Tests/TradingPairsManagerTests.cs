using Chipseky.MamkinInvestor.Domain;
using FluentAssertions;

namespace Chipseky.MamkinInvestor.Tests;

public class TradingPairsManagerTests
{
    [Fact]
    public void New_manager_have_empty_history()
    {
        var manager = new TradingPairsManager();
        
        manager.TradingPairs.Should().BeEmpty();
    }
    
    [Fact]
    public void Empty_trading_pairs_collection_raise_exception()
    {
        var manager = new TradingPairsManager();
        
        Action action = () =>
        {
            var tradingPairs = new Dictionary<string, TradingPairPriceChange>();
            manager.Update(tradingPairs);
        };

        action.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void First_market_data_doesnt_affect_trend()
    {
        var tradingPairsCount = 10;
        
        var manager = new TradingPairsManager();

        var tradingPairs = Enumerable.Range(1, tradingPairsCount)
            .ToDictionary(i => $"tr-pair-{i}", i => new TradingPairPriceChange((decimal)i / 100, i));

        manager.Update(tradingPairs);
        
        manager.TradingPairs.Should().HaveSameCount(tradingPairs);
        manager.TradingPairs.Should().ContainKeys(tradingPairs.Keys);
        manager.TradingPairs.First().Value.Trend.Should().BeEmpty();
    }
    
    [Fact]
    public void Add_the_same_trading_pairs_increase_queue_length()
    {
        var tradingPairsCount = 10;
        
        var manager = new TradingPairsManager();
        var tradingPairs = Enumerable.Range(1, tradingPairsCount)
            .ToDictionary(i => $"tr-pair-{i}", i => new TradingPairPriceChange((decimal)i / 100, i));
        manager.Update(tradingPairs);
        
        var nextTradingPairs = Enumerable.Range(1, tradingPairsCount)
            .ToDictionary(i => $"tr-pair-{i}", i => new TradingPairPriceChange((decimal)i / 100, i));
        manager.Update(nextTradingPairs);

        manager.TradingPairs.Should().ContainKeys(tradingPairs.Keys);
        manager.TradingPairs.First().Value.Trend.Should().HaveCount(1);
    }

    [Fact]
    public void Mark_missing_trading_pairs_outdated()
    {
        var tradingPairsCount = 10;

        var manager = new TradingPairsManager();
        var tradingPairs = Enumerable.Range(1, tradingPairsCount)
            .ToDictionary(i => $"tr-pair-{i}", i => new TradingPairPriceChange((decimal)i / 100, i));
        manager.Update(tradingPairs);
        
        var newTradingPairs = Enumerable.Range(3, tradingPairsCount)
            .ToDictionary(i => $"tr-pair-{i}", i => new TradingPairPriceChange((decimal)i / 100, i));
        manager.Update(newTradingPairs);

        manager.TradingPairs[$"tr-pair-{1}"].Outdated.Should().BeTrue();
        manager.TradingPairs[$"tr-pair-{2}"].Outdated.Should().BeTrue();
    }
    
    [Fact]
    public void Clean_up_remove_outdate_trading_pairs()
    {
        var tradingPairsCount = 5;

        var manager = new TradingPairsManager();
        var tradingPairs = Enumerable.Range(1, tradingPairsCount)
            .ToDictionary(i => $"tr-pair-{i}", i => new TradingPairPriceChange((decimal)i / 100, i));
        manager.Update(tradingPairs);
        
        var newTradingPairs = Enumerable.Range(3, tradingPairsCount)
            .ToDictionary(i => $"tr-pair-{i}", i => new TradingPairPriceChange((decimal)i / 100, i));
        manager.Update(newTradingPairs);
        
        manager.CleanUp();

        manager.TradingPairs.Should().NotContainKeys($"tr-pair-{1}", $"tr-pair-{2}");
    }
    
    [Fact]
    public void Save_new_trading_pairs()
    {
        var tradingPairsCount = 10;

        var manager = new TradingPairsManager();
        var tradingPairs = Enumerable.Range(1, tradingPairsCount)
            .ToDictionary(i => $"tr-pair-{i}", i => new TradingPairPriceChange((decimal)i / 100, i));
        manager.Update(tradingPairs);
        
        var newTradingPairs = Enumerable.Range(3, tradingPairsCount)
            .ToDictionary(i => $"tr-pair-{i}", i => new TradingPairPriceChange((decimal)i / 100, i));
        manager.Update(newTradingPairs);

        manager.TradingPairs.Should().ContainKey($"tr-pair-{11}");
        manager.TradingPairs.Should().ContainKey($"tr-pair-{12}");
    }
}