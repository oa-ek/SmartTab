using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTab.Core;

public class ShoppingCart
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
