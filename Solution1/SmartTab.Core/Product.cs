namespace SmartTab.Core;
using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    [Required]
    [Range(0.01, 1000000, ErrorMessage = "Ціна повинна бути більшою за 0")]
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }

    public int ManufacturerId { get; set; }
    public Manufacturer Manufacturer { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ProductType Type { get; set; } = ProductType.Component;

    public ICollection<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<BuildPart> PcParts { get; set; } = new List<BuildPart>();
    public ICollection<BuildPart> PartOfPcs { get; set; } = new List<BuildPart>();
}
