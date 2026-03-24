using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTab.Core;

public class OrderItemOption
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!;

    public int ComponentId { get; set; }
    public Product Component { get; set; } = null!;

    public decimal ComponentPrice { get; set; }
}