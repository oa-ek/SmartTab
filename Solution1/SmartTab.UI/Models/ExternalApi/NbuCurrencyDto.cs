using System.Text.Json.Serialization;

namespace SmartTab.UI.Models.ExternalApi;

public class NbuCurrencyDto
{
    [JsonPropertyName("r030")]
    public int CurrencyCode { get; set; }

    [JsonPropertyName("txt")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("rate")]
    public decimal Rate { get; set; }

    [JsonPropertyName("cc")]
    public string CurrencyAbbreviation { get; set; } = null!;

    [JsonPropertyName("exchangedate")]
    public string ExchangeDate { get; set; } = null!;
}
