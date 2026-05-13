using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SmartTab.UI.Models.Monobank;

namespace SmartTab.UI.Services;

public class MonobankService : IMonobankService
{
    private readonly HttpClient _httpClient;
    private readonly string _token;
    private readonly ILogger<MonobankService> _logger;

    public MonobankService(HttpClient httpClient, IConfiguration configuration, ILogger<MonobankService> logger)
    {
        _httpClient = httpClient;
        _token = configuration["Monobank:Token"]
            ?? throw new InvalidOperationException("Monobank token not configured");
        _logger = logger;
    }

    public async Task<MonobankInvoiceResponse?> CreateInvoiceAsync(
        long amountInKopiykas,
        string orderId,
        List<MonobankBasketItem> basketItems,
        string redirectUrl,
        string webHookUrl)
    {
        var request = new MonobankInvoiceRequest
        {
            Amount = amountInKopiykas,
            Ccy = 980,
            MerchantPaymInfo = new MonobankMerchantPaymInfo
            {
                Reference = orderId,
                Destination = $"Оплата замовлення #{orderId} — SmartTab Store",
                BasketOrder = basketItems
            },
            RedirectUrl = redirectUrl,
            WebHookUrl = webHookUrl,
            Validity = 3600,
            PaymentType = "debit"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/merchant/invoice/create")
        {
            Content = content
        };
        httpRequest.Headers.Add("X-Token", _token);

        try
        {
            var response = await _httpClient.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Monobank invoice creation failed: {StatusCode} - {Body}",
                    response.StatusCode, responseBody);
                return null;
            }

            var result = JsonSerializer.Deserialize<MonobankInvoiceResponse>(responseBody);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Monobank invoice");
            return null;
        }
    }

    public async Task<string?> GetInvoiceStatusAsync(string invoiceId)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get,
                $"api/merchant/invoice/status?invoiceId={invoiceId}");
            httpRequest.Headers.Add("X-Token", _token);

            var response = await _httpClient.SendAsync(httpRequest);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Monobank status check failed: {StatusCode} - {Body}",
                    response.StatusCode, body);
                return null;
            }

            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.GetProperty("status").GetString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Monobank invoice status");
            return null;
        }
    }
}
