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
