using Chipseky.MamkinInvestor.Domain;
using FluentAssertions;

namespace Chipseky.MamkinInvestor.Tests;

public class LotSizeFilterHelperTests
{
    [Fact]
    public void Zero_dont_have_decimals()
    {
        var decimals = LotSizeFilterHelper.GetDecimals(0);

        decimals.Should().Be(0);
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(1.1, 0)]
    [InlineData(2, 0)]
    [InlineData(0.1, 1)]
    [InlineData(0.01, 2)]
    [InlineData(0.00000001, 8)]
    [InlineData(0.001, 3)]
    public void Test(decimal basePrecision, int expectedDecimals)
    {
        var actualDecimals = LotSizeFilterHelper.GetDecimals(basePrecision);
        
        actualDecimals.Should().Be(expectedDecimals);
    }
}