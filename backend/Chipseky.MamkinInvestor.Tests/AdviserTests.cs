using Chipseky.MamkinInvestor.Domain;
using FluentAssertions;

namespace Chipseky.MamkinInvestor.Tests;

public class AdviserTests
{
    [Fact]
    public void Empty_trading_pairs_details_leads_to_nothing()
    {
        var advice = new Adviser().GiveAdvice(new TradingPairDetail(new TradingPairPriceChange(.1m, 1m)));

        advice.Should().Be(Advice.DoNothing);
    }
    
    [Theory]
    [InlineData(true, true)]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(false, true)]
    [InlineData(true, true, true, false)]
    [InlineData(true, true, true, false, true, true)]
    [InlineData(true, true, true, false, false, true)]
    [InlineData(false, true, false, true, true, false, true, false)]
    [InlineData(false, false, false)]
    public void Trends_not_enough_to_buy(params bool[] trendMap)
    {
        var tradingPairDetails = GenerateTradingPairDetail(trendMap);

        var advice = new Adviser().GiveAdvice(tradingPairDetails);

        advice.Should().Be(Advice.DoNothing);
    }
    
    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false, false, true, true, true)]
    [InlineData(false, true, true, true)]
    [InlineData(true, true, false, true, true, true)]
    [InlineData(false, false, false, true, true, true)]
    [InlineData(false, false, true, true, true)]
    public void Trends_enough_to_buy(params bool[] trendMap)
    {
        var tradingPairDetails = GenerateTradingPairDetail(trendMap);
        
        var advice = new Adviser().GiveAdvice(tradingPairDetails);

        advice.Should().Be(Advice.Buy);
    }
    
    [Fact]
    public void Dont_buy_held_pair_one_more_time()
    {
        var tradingPairDetails = GenerateTradingPairDetail(true, true, true);
        tradingPairDetails.MarkAsHeld();

        var advice = new Adviser().GiveAdvice(tradingPairDetails);

        advice.Should().Be(Advice.DoNothing);
    }

    [Fact]
    public void Fallen_pairs_must_be_sold()
    {
        var tradingPairDetail = GenerateTradingPairDetail(true, true, false, false);
        tradingPairDetail.MarkAsHeld();

        var advice = new Adviser().GiveAdvice(tradingPairDetail);

        advice.Should().Be(Advice.Sell);
    }

    [Fact]
    public void Outdated_and_held_pairs_must_be_sold()
    {
        var tradingPairDetail = GenerateTradingPairDetail(true, true, false, false);
        tradingPairDetail.MarkOutdated();
        tradingPairDetail.MarkAsHeld();
        
        var advice = new Adviser().GiveAdvice(tradingPairDetail);

        advice.Should().Be(Advice.Sell);
    }

    [Fact]
    public void Outdated_not_held_pairs_leads_nothing()
    {
        var tradingPairDetail = GenerateTradingPairDetail(true, true, false, false);
        tradingPairDetail.MarkOutdated();
        
        var advice = new Adviser().GiveAdvice(tradingPairDetail);

        advice.Should().Be(Advice.DoNothing);
    }

    private TradingPairDetail GenerateTradingPairDetail(params bool[] trendMap)
    {
        var tradingPairDetail = new TradingPairDetail(new TradingPairPriceChange(.5M, 1));

        var priceChangePercentage24H = .55m;
        var lastPrice = 1m;
        
        foreach (var increase in trendMap)
            if (increase)
            {
                priceChangePercentage24H *= 1.05m;
                lastPrice *= 1.05m;
                tradingPairDetail.Update(new TradingPairPriceChange(priceChangePercentage24H, lastPrice));
            }
            else
            {
                priceChangePercentage24H *= .95m;
                lastPrice *= .95m;
                tradingPairDetail.Update(new TradingPairPriceChange(priceChangePercentage24H, lastPrice));
            }

        return tradingPairDetail;
    }
}