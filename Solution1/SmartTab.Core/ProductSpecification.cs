namespace SmartTab.Core;

public class ProductSpecification
{
    public int Id { get; set; }
    
    // Назва характеристики (наприклад: "Socket", "Memory Type", "Frequency")
    public string Key { get; set; } = null!;
    
    // Значення (наприклад: "AM4", "DDR4", "3.6 GHz")
    public string Value { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
