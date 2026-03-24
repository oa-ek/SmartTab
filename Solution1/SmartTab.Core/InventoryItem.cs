using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SmartTab.Core;

public class InventoryItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string SerialNumber { get; set; } = null!;

    public bool IsSold { get; set; } = false;

    public int? OrderItemId { get; set; }
    public OrderItem? OrderItem { get; set; }

    public int? OrderItemOptionId { get; set; }
    public OrderItemOption? OrderItemOption { get; set; }
}