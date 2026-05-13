namespace SmartTab.UI.Models.ExternalApi;

public class ProductEnrichedViewModel
{
    // Локальні дані з БД
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal PriceUah { get; set; }
    public string? ImageUrl { get; set; }
    public string? CategoryName { get; set; }
    public string? ManufacturerName { get; set; }

    // Дані з NBU API — ціна в інших валютах
    public decimal? PriceUsd { get; set; }
    public decimal? PriceEur { get; set; }
    public decimal? UsdRate { get; set; }
    public decimal? EurRate { get; set; }
    public string? RateDate { get; set; }

    // Дані з REST Countries API — країна виробника
    public string? CountryName { get; set; }
    public string? CountryOfficialName { get; set; }
    public string? CountryFlagUrl { get; set; }
    public string? CountryCapital { get; set; }
    public string? CountryRegion { get; set; }
    public long? CountryPopulation { get; set; }
    public string? CountryCurrency { get; set; }
    public string? CountryLanguages { get; set; }

    // Ланцюжок: REST Countries → NBU (валюта країни виробника)
    public string? ManufacturerCurrencyCode { get; set; }
    public decimal? ManufacturerCurrencyRate { get; set; }
    public decimal? PriceInManufacturerCurrency { get; set; }

    // Статус завантаження
    public bool CurrencyAvailable { get; set; }
    public bool CountryAvailable { get; set; }
    public bool ManufacturerCurrencyAvailable { get; set; }
}
