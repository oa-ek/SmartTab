namespace SmartTab.UI.Models;

public class CreateOrderRequest
{
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateBalanceRequest
{
    public decimal Balance { get; set; }
}

public class ReceiptItem
{
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public List<string> SerialNumbers { get; set; } = new();
}
