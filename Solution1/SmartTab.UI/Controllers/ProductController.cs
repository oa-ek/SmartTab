using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartTab.Core;
using SmartTab.Data;

namespace SmartTab.UI.Controllers;

public class ProductsController : Controller
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Products
    public async Task<IActionResult> Index()
    {
        // Підтягуємо Категорію та Виробника, щоб виводити їхні назви в таблиці
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .ToListAsync();

        return View(products);
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null) return NotFound();

        return View(product);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        // Передаємо списки для <select> у HTML форму
        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
        ViewBag.ManufacturerId = new SelectList(_context.Manufacturers, "Id", "Name");
        return View();
    }

    // POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Якщо помилка валідації - заново генеруємо списки перед поверненням форми
        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        ViewBag.ManufacturerId = new SelectList(_context.Manufacturers, "Id", "Name", product.ManufacturerId);
        return View(product);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // Передаємо списки, але четвертим параметром вказуємо поточне значення товару, 
        // щоб воно було обране у випадаючому списку за замовчуванням
        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        ViewBag.ManufacturerId = new SelectList(_context.Manufacturers, "Id", "Name", product.ManufacturerId);
        return View(product);
    }

    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        ViewBag.ManufacturerId = new SelectList(_context.Manufacturers, "Id", "Name", product.ManufacturerId);
        return View(product);
    }

    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null) return NotFound();

        return View(product);
    }
    // POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}