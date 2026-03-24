using System.ComponentModel.DataAnnotations;

namespace SmartTab.Core;

public class Manufacturer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Назва виробника є обов'язковою")]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(255)]
    public string? WebsiteUrl { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}