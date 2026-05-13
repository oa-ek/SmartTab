using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using SmartTab.UI.Models.ExternalApi;

namespace SmartTab.UI.Services;

public class CurrencyApiService : ICurrencyApiService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CurrencyApiService> _logger;
    private const string CacheKey = "nbu_exchange_rates";

    public CurrencyApiService(HttpClient httpClient, IMemoryCache cache, ILogger<CurrencyApiService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<NbuCurrencyDto>?> GetExchangeRatesAsync()
    {
        if (_cache.TryGetValue(CacheKey, out List<NbuCurrencyDto>? cached))
            return cached;

        try
        {
            var response = await _httpClient.GetAsync("NBUStatService/v1/statdirectory/exchange?json");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var rates = JsonSerializer.Deserialize<List<NbuCurrencyDto>>(json);

            if (rates != null)
                _cache.Set(CacheKey, rates, TimeSpan.FromMinutes(30));

            return rates;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "NBU API недоступний");
            return null;
        }
    }

    public async Task<decimal?> GetRateAsync(string currencyCode)
    {
        var rates = await GetExchangeRatesAsync();
        return rates?.FirstOrDefault(r =>
            r.CurrencyAbbreviation.Equals(currencyCode, StringComparison.OrdinalIgnoreCase))?.Rate;
    }
}
