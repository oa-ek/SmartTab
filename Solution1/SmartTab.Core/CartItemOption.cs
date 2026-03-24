using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTab.Core;

public class CartItemOption
{
    public int Id { get; set; }

    public int CartItemId { get; set; }
    public CartItem CartItem { get; set; } = null!;

    public int ComponentId { get; set; }
    public Product Component { get; set; } = null!;
}