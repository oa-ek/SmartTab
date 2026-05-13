using SmartTab.UI.Models.Monobank;

namespace SmartTab.UI.Services;

public interface IMonobankService
{
    Task<MonobankInvoiceResponse?> CreateInvoiceAsync(long amountInKopiykas, string orderId, List<MonobankBasketItem> basketItems, string redirectUrl, string webHookUrl);
}
