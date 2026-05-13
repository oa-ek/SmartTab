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

    public async Task<decimal?> GetRateWithFallbackAsync(string currencyCode)
    {
        var directRate = await GetRateAsync(currencyCode);
        if (directRate.HasValue) return directRate;

        var cacheKey = $"fallback_rate_{currencyCode.ToUpper()}";
        if (_cache.TryGetValue(cacheKey, out decimal cachedRate))
            return cachedRate;

        try
        {
            var usdToUah = await GetRateAsync("USD");
            if (!usdToUah.HasValue) return null;

            var response = await _httpClient.GetAsync(
                $"https://open.er-api.com/v6/latest/USD");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var rates = doc.RootElement.GetProperty("rates");

            if (!rates.TryGetProperty(currencyCode.ToUpper(), out var targetRate))
                return null;

            var usdToTarget = targetRate.GetDecimal();
            if (usdToTarget == 0) return null;

            var rateToUah = usdToUah.Value / usdToTarget;
            _cache.Set(cacheKey, rateToUah, TimeSpan.FromMinutes(30));

            return rateToUah;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Fallback currency conversion failed for {Code}", currencyCode);
            return null;
        }
    }
}
