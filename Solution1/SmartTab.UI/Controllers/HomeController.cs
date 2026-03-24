using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SmartTab.Data;
using SmartTab.Core;
using System.IO;

namespace SmartTab.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HomeController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index() => View();
        public IActionResult Admin() => View();

        [HttpGet("api/components")]
        public async Task<IActionResult> GetComponents()
        {
            var components = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Type == ProductType.Component)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.StockCount,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : "Невідомо"
                })
                .OrderBy(p => p.CategoryName)
                .ThenBy(p => p.Name)
                .ToListAsync();

            return Ok(components);
        }

        [HttpGet("api/products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Specifications)
                .Include(p => p.Category)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.ImageUrl,
                    p.StockCount,
                    CategoryId = p.CategoryId,
                    ProductType = (int)p.Type,
                    CategoryName = p.Category != null ? p.Category.Name : "Невідомо",
                    Specs = p.Specifications.Select(s => new { s.Name, s.Value })
                })
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return Ok(products);
        }

        [HttpPost("api/products")]
        public async Task<IActionResult> AddProduct([FromForm] string name, [FromForm] decimal price, [FromForm] int categoryId, [FromForm] int productType, [FromForm] int stockCount, [FromForm] string specsJson, IFormFile? image)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || price <= 0 || categoryId <= 0)
                    return BadRequest("Невалідні дані товару.");

                var product = new Product
                {
                    Name = name.Trim(),
                    Price = price,
                    CategoryId = categoryId,
                    Type = (ProductType)productType,
                    StockCount = stockCount
                };

                if (image != null && image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = "/uploads/" + uniqueFileName;
                }

                if (!string.IsNullOrWhiteSpace(specsJson))
                {
                    var specsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(specsJson);
                    if (specsDict != null)
                    {
                        foreach (var spec in specsDict)
                        {
                            if (!string.IsNullOrWhiteSpace(spec.Value))
                            {
                                product.Specifications.Add(new ProductSpecification { Name = spec.Key, Value = spec.Value.Trim() });
                            }
                        }
                    }
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Товар успішно додано" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("api/products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPut("api/products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] string name, [FromForm] decimal price, [FromForm] int categoryId, [FromForm] int productType, [FromForm] int stockCount, [FromForm] string specsJson, IFormFile? image)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Specifications)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return NotFound("Товар не знайдено");

                if (string.IsNullOrWhiteSpace(name) || price <= 0 || categoryId <= 0)
                    return BadRequest("Невалідні дані товару.");

                product.Name = name.Trim();
                product.Price = price;
                product.CategoryId = categoryId;
                product.Type = (ProductType)productType;
                product.StockCount = stockCount;

                if (image != null && image.Length > 0)
                {
                    // Delete old image
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_environment.WebRootPath, product.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = "/uploads/" + uniqueFileName;
                }

                // Update specifications
                product.Specifications.Clear();
                if (!string.IsNullOrWhiteSpace(specsJson))
                {
                    var specsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(specsJson);
                    if (specsDict != null)
                    {
                        foreach (var spec in specsDict)
                        {
                            if (!string.IsNullOrWhiteSpace(spec.Value))
                            {
                                product.Specifications.Add(new ProductSpecification { Name = spec.Key, Value = spec.Value.Trim() });
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Товар успішно оновлено" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}