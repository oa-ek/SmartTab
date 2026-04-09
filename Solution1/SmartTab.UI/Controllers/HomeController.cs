using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTab.Core;
using SmartTab.Data;
using SmartTab.UI;
using SmartTab.UI.Models;

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
        public IActionResult Privacy() => View();

        public IActionResult ProductPage(int id) => View();
        public IActionResult ConfigPC() => View();

        [Authorize(Roles = "Admin")]
        public IActionResult Admin() => View();

        // API для отримання одного товару за ID
        [HttpGet("api/products/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Specifications)
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.ImageUrl,
                    p.StockCount,
                    CategoryId = p.CategoryId,
                    ManufacturerId = p.ManufacturerId,
                    ProductType = (int)p.Type,
                    CategoryName = p.Category != null ? p.Category.Name : "Невідомо",
                    ManufacturerName = p.Manufacturer != null ? p.Manufacturer.Name : "Не вказано",
                    Specs = p.Specifications.Select(s => new { s.Name, s.Value })
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpGet("api/inventory/{productId}")]
        public async Task<IActionResult> GetInventoryItems(int productId)
        {
            var inventoryItems = await _context.InventoryItems
                .Where(i => i.ProductId == productId)
                .Select(i => new {
                    i.Id,
                    i.SerialNumber,
                    i.IsSold,
                    OrderItemId = i.OrderItemId
                })
                .OrderByDescending(i => i.Id)
                .ToListAsync();

            return Ok(inventoryItems);
        }

        [HttpGet("api/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            return Ok(categories);
        }

        [HttpPost("api/categories")]
        public async Task<IActionResult> AddCategory([FromForm] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest("Назва категорії є обов'язковою");

                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim());

                if (existingCategory != null)
                    return BadRequest($"Категорія з назвою '{name}' вже існує");

                var category = new Category { Name = name.Trim() };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Категорію успішно додано", category });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut("api/categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] string name)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    return NotFound("Категорію не знайдено");

                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest("Назва категорії є обов'язковою");

                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim() && c.Id != id);

                if (existingCategory != null)
                    return BadRequest($"Категорія з назвою '{name}' вже існує");

                category.Name = name.Trim();
                await _context.SaveChangesAsync();

                return Ok(new { message = "Категорію успішно оновлено", category });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("api/categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return NotFound("Категорію не знайдено");

                if (category.Products.Any())
                    return BadRequest("Неможливо видалити категорію, оскільки до неї прив'язані товари");

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Категорію успішно видалено" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("api/manufacturers")]
        public async Task<IActionResult> GetManufacturers()
        {
            var manufacturers = await _context.Manufacturers
                .OrderBy(m => m.Name)
                .ToListAsync();
            return Ok(manufacturers);
        }

        [HttpPost("api/manufacturers")]
        public async Task<IActionResult> AddManufacturer([FromForm] string name, [FromForm] string? description, [FromForm] string? websiteUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest("Назва виробника є обов'язковою");

                var existingManufacturer = await _context.Manufacturers
                    .FirstOrDefaultAsync(m => m.Name.ToLower().Trim() == name.ToLower().Trim());

                if (existingManufacturer != null)
                    return BadRequest($"Виробник з назвою '{name}' вже існує");

                var manufacturer = new Manufacturer
                {
                    Name = name.Trim(),
                    Description = description?.Trim(),
                    WebsiteUrl = websiteUrl?.Trim()
                };

                _context.Manufacturers.Add(manufacturer);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Виробника успішно додано", manufacturer });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut("api/manufacturers/{id}")]
        public async Task<IActionResult> UpdateManufacturer(int id, [FromForm] string name, [FromForm] string? description, [FromForm] string? websiteUrl)
        {
            try
            {
                var manufacturer = await _context.Manufacturers.FindAsync(id);
                if (manufacturer == null)
                    return NotFound("Виробника не знайдено");

                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest("Назва виробника є обов'язковою");

                var existingManufacturer = await _context.Manufacturers
                    .FirstOrDefaultAsync(m => m.Name.ToLower().Trim() == name.ToLower().Trim() && m.Id != id);

                if (existingManufacturer != null)
                    return BadRequest($"Виробник з назвою '{name}' вже існує");

                manufacturer.Name = name.Trim();
                manufacturer.Description = description?.Trim();
                manufacturer.WebsiteUrl = websiteUrl?.Trim();

                await _context.SaveChangesAsync();

                return Ok(new { message = "Виробника успішно оновлено", manufacturer });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("api/manufacturers/{id}")]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {
            try
            {
                var manufacturer = await _context.Manufacturers
                    .Include(m => m.Products)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (manufacturer == null)
                    return NotFound("Виробника не знайдено");

                if (manufacturer.Products.Any())
                    return BadRequest("Неможливо видалити виробника, оскільки до нього прив'язані товари");

                _context.Manufacturers.Remove(manufacturer);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Виробника успішно видалено" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("api/products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Specifications)
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.ImageUrl,
                    p.StockCount,
                    CategoryId = p.CategoryId,
                    ManufacturerId = p.ManufacturerId,
                    ProductType = (int)p.Type,
                    CategoryName = p.Category != null ? p.Category.Name : "Невідомо",
                    ManufacturerName = p.Manufacturer != null ? p.Manufacturer.Name : "Не вказано",
                    Specs = p.Specifications.Select(s => new { s.Name, s.Value })
                })
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return Ok(products);
        }

        [HttpPost("api/products")]
        public async Task<IActionResult> AddProduct([FromForm] string name, [FromForm] decimal price, [FromForm] int categoryId, [FromForm] int productType, [FromForm] int stockCount, [FromForm] int initialQuantity, [FromForm] int manufacturerId, [FromForm] string specsJson, IFormFile? image)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || price <= 0 || categoryId <= 0)
                    return BadRequest("Невалідні дані товару.");

                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Name.ToLower().Trim() == name.ToLower().Trim());

                if (existingProduct != null)
                    return BadRequest($"Товар з назвою '{name}' вже існує. Будь ласка, використайте іншу назву.");

                var product = new Product
                {
                    Name = name.Trim(),
                    Price = price,
                    CategoryId = categoryId,
                    ManufacturerId = manufacturerId > 0 ? manufacturerId : null,
                    Type = (ProductType)productType,
                    StockCount = stockCount,
                    InitialQuantity = initialQuantity
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

                if (initialQuantity > 0)
                {
                    var inventoryItems = new List<InventoryItem>();

                    for (int i = 0; i < initialQuantity; i++)
                    {
                        inventoryItems.Add(new InventoryItem
                        {
                            ProductId = product.Id,
                            SerialNumber = $"SN-{product.Name.Replace(" ", "").ToUpper()}-{Guid.NewGuid().ToString().Substring(0, 6)}",
                            IsSold = false
                        });
                    }

                    _context.InventoryItems.AddRange(inventoryItems);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = $"Товар успішно додано. Згенеровано {initialQuantity} серійних номерів." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("api/products/{id}/add-serials")]
        public async Task<IActionResult> AddSerialNumbers(int id, [FromForm] int quantity)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound("Товар не знайдено");

                if (quantity <= 0)
                    return BadRequest("Невалідна кількість товару.");

                product.StockCount += quantity;

                var inventoryItems = new List<InventoryItem>();

                for (int i = 0; i < quantity; i++)
                {
                    inventoryItems.Add(new InventoryItem
                    {
                        ProductId = product.Id,
                        SerialNumber = $"SN-{product.Name.Replace(" ", "").ToUpper()}-{Guid.NewGuid().ToString().Substring(0, 6)}",
                        IsSold = false
                    });
                }

                _context.InventoryItems.AddRange(inventoryItems);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"До товару '{product.Name}' додано {quantity} серійних номерів." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("api/products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.InventoryItems)
                    .Include(p => p.Specifications)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return NotFound("Товар не знайдено");

                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.InventoryItems.RemoveRange(product.InventoryItems);
                _context.Set<ProductSpecification>().RemoveRange(product.Specifications);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Товар успішно видалено" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut("api/products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] string name, [FromForm] decimal price, [FromForm] int categoryId, [FromForm] int productType, [FromForm] int stockCount, [FromForm] int manufacturerId, [FromForm] int initialQuantity, [FromForm] string specsJson, IFormFile? image)
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

                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Name.ToLower().Trim() == name.ToLower().Trim() && p.Id != id);

                if (existingProduct != null)
                    return BadRequest($"Товар з назвою '{name}' вже існує. Будь ласка, використайте іншу назву.");

                product.Name = name.Trim();
                product.Price = price;
                product.CategoryId = categoryId;
                product.ManufacturerId = manufacturerId > 0 ? manufacturerId : null;
                product.Type = (ProductType)productType;
                product.StockCount = stockCount;

                if (image != null && image.Length > 0)
                {
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

                _context.RemoveRange(product.Specifications);

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

                if (initialQuantity > 0)
                {
                    var inventoryItems = new List<InventoryItem>();
                    for (int i = 0; i < initialQuantity; i++)
                    {
                        inventoryItems.Add(new InventoryItem
                        {
                            ProductId = product.Id,
                            SerialNumber = $"SN-{product.Name.Replace(" ", "").ToUpper()}-{Guid.NewGuid().ToString().Substring(0, 6)}",
                            IsSold = false
                        });
                    }
                    _context.InventoryItems.AddRange(inventoryItems);

                    product.StockCount += initialQuantity;
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Товар успішно оновлено" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        // GET: /api/balance
        [HttpGet("api/balance")]
        [Authorize]
        public async Task<IActionResult> GetBalance()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();
            return Ok(new { balance = user.Balance });
        }

        // POST: /api/orders
        [HttpPost("api/orders")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (request.Items == null || request.Items.Count == 0)
                    return BadRequest(new { error = "Кошик порожній" });

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound(new { error = "Користувача не знайдено" });

                // Завантажуємо продукти з інвентарем
                var productIds = request.Items.Select(i => i.ProductId).ToList();
                var products = await _context.Products
                    .Include(p => p.InventoryItems.Where(inv => !inv.IsSold))
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                // Перевіряємо наявність кожного товару
                decimal totalPrice = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in request.Items)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product == null)
                        return BadRequest(new { error = $"Товар з ID {item.ProductId} не знайдено" });

                    if (product.StockCount < item.Quantity)
                        return BadRequest(new { error = $"Недостатньо '{product.Name}' на складі. Доступно: {product.StockCount}" });

                    totalPrice += product.Price * item.Quantity;
                    orderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });
                }

                // Перевіряємо баланс
                if (user.Balance < totalPrice)
                    return BadRequest(new { error = $"Недостатньо коштів. Потрібно: {totalPrice} ₴, баланс: {user.Balance} ₴" });

                // Створюємо замовлення
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    Price = totalPrice,
                    Status = "Оплачено"
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Додаємо позиції замовлення
                foreach (var oi in orderItems)
                {
                    oi.OrderId = order.Id;
                    _context.OrderItems.Add(oi);
                }
                await _context.SaveChangesAsync();

                // Оновлюємо кількість на складі та списуємо баланс
                foreach (var oi in orderItems)
                {
                    var product = products.First(p => p.Id == oi.ProductId);
                    product.StockCount -= oi.Quantity;
                }
                user.Balance -= totalPrice;
                await _context.SaveChangesAsync();

                // Прив'язуємо InventoryItems (якщо є)
                foreach (var oi in orderItems)
                {
                    var product = products.First(p => p.Id == oi.ProductId);
                    var availableInventory = product.InventoryItems
                        .Where(inv => !inv.IsSold)
                        .Take(oi.Quantity)
                        .ToList();

                    foreach (var inv in availableInventory)
                    {
                        inv.IsSold = true;
                        inv.OrderItemId = oi.Id;
                    }
                }
                if (_context.ChangeTracker.HasChanges())
                    await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok(new { success = true, orderId = order.Id, newBalance = user.Balance });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { error = $"Помилка: {ex.Message}" });
            }
        }

        // GET: /api/orders/my
        [HttpGet("api/orders/my")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.Price,
                    o.Status,
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.Product.Name,
                        oi.Product.ImageUrl,
                        oi.Quantity,
                        oi.UnitPrice
                    })
                })
                .ToListAsync();

            return Ok(orders);
        }

        // GET: /api/users (Admin only)
        [HttpGet("api/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Balance,
                    Role = u.Role.Name
                })
                .ToListAsync();
            return Ok(users);
        }

        // PUT: /api/users/{id}/balance (Admin only)
        [HttpPut("api/users/{id}/balance")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserBalance(int id, [FromBody] UpdateBalanceRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { error = "Користувача не знайдено" });

            user.Balance = request.Balance;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, newBalance = user.Balance });
        }

        // DELETE: /api/orders/{id} (Admin only) — видалення порожніх замовлень
        [HttpDelete("api/orders/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound(new { error = "Замовлення не знайдено" });

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}