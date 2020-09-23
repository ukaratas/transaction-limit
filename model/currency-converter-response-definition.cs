using Newtonsoft.Json;

public class CurrencyConverterResponseDefinition
{
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("buyAmount")]
    public decimal BuyAmount { get; set; }

    [JsonProperty("sellAmount")]
    public decimal SellAmount { get; set; }
}