using System.Text.Json.Serialization;

namespace SmartTab.UI.Models.Monobank;

public class MonobankInvoiceRequest
{
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("ccy")]
    public int Ccy { get; set; } = 980; // UAH

    [JsonPropertyName("merchantPaymInfo")]
    public MonobankMerchantPaymInfo? MerchantPaymInfo { get; set; }

    [JsonPropertyName("redirectUrl")]
    public string RedirectUrl { get; set; } = null!;

    [JsonPropertyName("webHookUrl")]
    public string WebHookUrl { get; set; } = null!;

    [JsonPropertyName("validity")]
    public int Validity { get; set; } = 3600; // 1 година

    [JsonPropertyName("paymentType")]
    public string PaymentType { get; set; } = "debit";
}

public class MonobankMerchantPaymInfo
{
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = null!;

    [JsonPropertyName("destination")]
    public string Destination { get; set; } = null!;

    [JsonPropertyName("basketOrder")]
    public List<MonobankBasketItem>? BasketOrder { get; set; }
}

public class MonobankBasketItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("qty")]
    public int Qty { get; set; }

    [JsonPropertyName("sum")]
    public long Sum { get; set; }

    [JsonPropertyName("total")]
    public long Total { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "шт.";
}
