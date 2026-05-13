using System.Text.Json.Serialization;

namespace SmartTab.UI.Models.ExternalApi;

public class RestCountryDto
{
    [JsonPropertyName("name")]
    public CountryName? Name { get; set; }

    [JsonPropertyName("capital")]
    public List<string>? Capital { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("population")]
    public long Population { get; set; }

    [JsonPropertyName("flags")]
    public CountryFlags? Flags { get; set; }

    [JsonPropertyName("currencies")]
    public Dictionary<string, CurrencyInfo>? Currencies { get; set; }

    [JsonPropertyName("languages")]
    public Dictionary<string, string>? Languages { get; set; }
}

public class CountryName
{
    [JsonPropertyName("common")]
    public string? Common { get; set; }

    [JsonPropertyName("official")]
    public string? Official { get; set; }
}

public class CountryFlags
{
    [JsonPropertyName("png")]
    public string? Png { get; set; }

    [JsonPropertyName("svg")]
    public string? Svg { get; set; }
}

public class CurrencyInfo
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
}
