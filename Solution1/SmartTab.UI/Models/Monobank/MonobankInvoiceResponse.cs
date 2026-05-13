using System.Text.Json.Serialization;

namespace SmartTab.UI.Models.Monobank;

public class MonobankInvoiceResponse
{
    [JsonPropertyName("invoiceId")]
    public string InvoiceId { get; set; } = null!;

    [JsonPropertyName("pageUrl")]
    public string PageUrl { get; set; } = null!;
}
