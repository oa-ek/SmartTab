using System.ComponentModel.DataAnnotations;

namespace SmartTab.Core;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Назва категорії є обов'язковою")]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    
    public ICollection<Product> Products { get; set; } = new List<Product>();
}