using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTab.Core;

public class CartItem
{
    public int Id { get; set; }

    public int ShoppingCartId { get; set; }
    public ShoppingCart ShoppingCart { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}