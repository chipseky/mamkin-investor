using System.Text.Json.Serialization;

namespace Mamkin.In.Domain;

public class Forecast
{
    public Guid ForecastId { get; private set; }
        
    [JsonPropertyName("h")]
    public decimal HeightPrice { get; private set; }
    
    [JsonPropertyName("l")]
    public decimal LowPrice { get; private set; }
    
    [JsonPropertyName("p_up")]
    public double HeightPriceProbability { get; private set; }
    
    [JsonPropertyName("p_down")]
    public double LowPriceProbability { get; private set; }

    [JsonPropertyName("error")]
    public string? Error { get; private set; }

    public Forecast(decimal heightPrice, decimal lowPrice, double heightPriceProbability, double lowPriceProbability)
    {
        HeightPrice = heightPrice;
        LowPrice = lowPrice;
        HeightPriceProbability = heightPriceProbability;
        LowPriceProbability = lowPriceProbability;
        ForecastId = Guid.NewGuid();
    }
}