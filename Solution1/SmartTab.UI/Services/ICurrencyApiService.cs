using SmartTab.UI.Models.ExternalApi;

namespace SmartTab.UI.Services;

public interface ICurrencyApiService
{
    Task<List<NbuCurrencyDto>?> GetExchangeRatesAsync();
    Task<decimal?> GetRateAsync(string currencyCode);
    Task<decimal?> GetRateWithFallbackAsync(string currencyCode);
}
