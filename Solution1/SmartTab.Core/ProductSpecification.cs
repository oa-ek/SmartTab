// SmartTab.Core/ProductSpecification.cs
namespace SmartTab.Core
{
    public class ProductSpecification
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}