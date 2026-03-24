using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTab.Core;
using SmartTab.Data;

namespace SmartTab.UI.Controllers;

public class CategoriesController : Controller
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Categories (Список)
    public async Task<IActionResult> Index()
    {
        return View(await _context.Categories.ToListAsync());
    }

    // GET: Categories/Details/5 (Перегляд однієї)
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
        if (category == null) return NotFound();

        return View(category);
    }

    // GET: Categories/Create (Форма створення)
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categories/Create (Збереження нової)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: Categories/Edit/5 (Форма редагування)
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        return View(category);
    }

    // POST: Categories/Edit/5 (Збереження змін)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: Categories/Delete/5 (Форма підтвердження видалення)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
        if (category == null) return NotFound();

        return View(category);
    }

    // POST: Categories/Delete/5 (Фізичне видалення)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}