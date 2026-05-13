using SmartTab.UI.Models.ExternalApi;

namespace SmartTab.UI.Services;

public interface ICountryApiService
{
    Task<RestCountryDto?> GetCountryInfoAsync(string countryName);
}
