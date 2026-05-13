using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTab.Core;
using SmartTab.Data;

namespace SmartTab.UI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsApiController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Отримати всі товари
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductApiDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? category = null, [FromQuery] string? manufacturer = null)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category.Name.Contains(category));

        if (!string.IsNullOrEmpty(manufacturer))
            query = query.Where(p => p.Manufacturer != null && p.Manufacturer.Name.Contains(manufacturer));

        var products = await query.Select(p => new ProductApiDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            StockCount = p.StockCount,
            CategoryName = p.Category.Name,
            ManufacturerName = p.Manufacturer != null ? p.Manufacturer.Name : null,
            ManufacturerCountry = p.Manufacturer != null ? p.Manufacturer.Country : null
        }).ToListAsync();

        return Ok(products);
    }

    /// <summary>
    /// Отримати товар за ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductApiDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .Include(p => p.Specifications)
            .Where(p => p.Id == id)
            .Select(p => new ProductApiDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                StockCount = p.StockCount,
                CategoryName = p.Category.Name,
                ManufacturerName = p.Manufacturer != null ? p.Manufacturer.Name : null,
                ManufacturerCountry = p.Manufacturer != null ? p.Manufacturer.Country : null,
                Specifications = p.Specifications.Select(s => new SpecDto { Name = s.Name, Value = s.Value }).ToList()
            })
            .FirstOrDefaultAsync();

        if (product == null)
            return NotFound(new { error = "Товар не знайдено" });

        return Ok(product);
    }

    /// <summary>
    /// Створити новий товар
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductApiDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductApiDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            StockCount = dto.StockCount,
            CategoryId = dto.CategoryId,
            ManufacturerId = dto.ManufacturerId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var result = new ProductApiDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            StockCount = product.StockCount
        };

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, result);
    }

    /// <summary>
    /// Оновити товар
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductApiDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductApiDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound(new { error = "Товар не знайдено" });

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.ImageUrl = dto.ImageUrl;
        product.StockCount = dto.StockCount;
        product.CategoryId = dto.CategoryId;
        product.ManufacturerId = dto.ManufacturerId;

        await _context.SaveChangesAsync();

        return Ok(new ProductApiDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            StockCount = product.StockCount
        });
    }

    /// <summary>
    /// Видалити товар
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound(new { error = "Товар не знайдено" });

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

// --- DTO ---
public class ProductApiDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int StockCount { get; set; }
    public string? CategoryName { get; set; }
    public string? ManufacturerName { get; set; }
    public string? ManufacturerCountry { get; set; }
    public List<SpecDto>? Specifications { get; set; }
}

public class SpecDto
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
}

public class CreateProductApiDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int StockCount { get; set; }
    public int CategoryId { get; set; }
    public int? ManufacturerId { get; set; }
}

public class UpdateProductApiDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int StockCount { get; set; }
    public int CategoryId { get; set; }
    public int? ManufacturerId { get; set; }
}
