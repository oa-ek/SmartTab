using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using SmartTab.UI.Models.ExternalApi;

namespace SmartTab.UI.Services;

public class CountryApiService : ICountryApiService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CountryApiService> _logger;

    public CountryApiService(HttpClient httpClient, IMemoryCache cache, ILogger<CountryApiService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<RestCountryDto?> GetCountryInfoAsync(string countryName)
    {
        var cacheKey = $"country:{countryName.ToLower()}";

        if (_cache.TryGetValue(cacheKey, out RestCountryDto? cached))
            return cached;

        try
        {
            var response = await _httpClient.GetAsync($"v3.1/name/{Uri.EscapeDataString(countryName)}?fields=name,capital,region,population,flags,currencies,languages");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var countries = JsonSerializer.Deserialize<List<RestCountryDto>>(json);
            var country = countries?.FirstOrDefault();

            if (country != null)
                _cache.Set(cacheKey, country, TimeSpan.FromHours(24));

            return country;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "REST Countries API недоступний для країни: {Country}", countryName);
            return null;
        }
    }
}
