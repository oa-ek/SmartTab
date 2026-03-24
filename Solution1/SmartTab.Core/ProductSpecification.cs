namespace SmartTab.Core;

public class ProductSpecification
{
    public int Id { get; set; }
    
    // Назва характеристики ("Socket", "Memory Type")
    public string Key { get; set; } = null!;
    
    // Значення ("AM4", "DDR4")
    public string Value { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
