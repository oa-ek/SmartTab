using System.Text.Json.Serialization;

namespace SmartTab.UI.Models.Monobank;

public class MonobankWebhookPayload
{
    [JsonPropertyName("invoiceId")]
    public string InvoiceId { get; set; } = null!;

    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("failureReason")]
    public string? FailureReason { get; set; }

    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("ccy")]
    public int Ccy { get; set; }

    [JsonPropertyName("finalAmount")]
    public long FinalAmount { get; set; }

    [JsonPropertyName("createdDate")]
    public string? CreatedDate { get; set; }

    [JsonPropertyName("modifiedDate")]
    public string? ModifiedDate { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }
}
