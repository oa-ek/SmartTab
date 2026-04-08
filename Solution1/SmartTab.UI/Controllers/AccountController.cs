using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTab.Core;
using SmartTab.Data;
using SmartTab.UI.Models;

namespace SmartTab.UI.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return RedirectToAction("Privacy", "Home");
    }

    // POST: /Account/Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .ToDictionary(k => k.Key, v => v.Value!.Errors.First().ErrorMessage);
                return Json(new { success = false, errors = validationErrors });
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

            if (existingUser != null)
                return Json(new { success = false, errors = new { Email = "Цей email вже зареєстрований" } });

            var nameParts = model.FullName.Trim().Split(' ', 2);
            var lastName = nameParts.Length > 1 ? nameParts[0] : "";
            var firstName = nameParts.Length > 1 ? nameParts[1] : nameParts[0];

            var isFirstUser = !await _context.Users.AnyAsync();

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = model.Email.ToLower().Trim(),
                PhoneNumber = model.PhoneNumber?.Trim(),
                Password = HashPassword(model.Password),
                RoleId = isFirstUser ? 1 : 2,
                IsActive = true,
                RegistrationDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SignInUser(user, isFirstUser ? "Admin" : "Customer", isPersistent: false);

            return Json(new { success = true, redirectUrl = "/" });
        }
        catch (Exception ex)
        {
            // У разі падіння сервера повертаємо текст помилки на фронтенд
            return Json(new { success = false, error = $"[DEBUG ПОМИЛКА]: {ex.InnerException?.Message ?? ex.Message}" });
        }
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        try
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, error = "Заповніть всі поля коректно" });

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

            if (user == null || !VerifyPassword(model.Password, user.Password))
                return Json(new { success = false, error = "Невірний email або пароль" });

            if (!user.IsActive)
                return Json(new { success = false, error = "Акаунт заблоковано. Зверніться до адміністратора." });

            await SignInUser(user, user.Role.Name, isPersistent: model.RememberMe);

            var redirect = (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                ? returnUrl : "/";

            return Json(new { success = true, redirectUrl = redirect });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = $"[DEBUG] {ex.GetType().Name}: {ex.Message}" });
        }
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/Profile
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound();

        var model = new ProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleName = user.Role.Name,
            RegistrationDate = user.RegistrationDate
        };

        return View(model);
    }

    // POST: /Account/Profile
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (string.IsNullOrEmpty(model.NewPassword))
        {
            ModelState.Remove("NewPassword");
            ModelState.Remove("ConfirmNewPassword");
            ModelState.Remove("CurrentPassword");
        }

        if (!ModelState.IsValid)
            return View(model);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound();

        var emailTaken = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == model.Email.ToLower() && u.Id != userId);

        if (emailTaken)
        {
            ModelState.AddModelError("Email", "Цей email вже використовується");
            return View(model);
        }

        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            if (string.IsNullOrEmpty(model.CurrentPassword) || !VerifyPassword(model.CurrentPassword, user.Password))
            {
                ModelState.AddModelError("CurrentPassword", "Невірний поточний пароль");
                return View(model);
            }

            user.Password = HashPassword(model.NewPassword);
        }

        user.FirstName = model.FirstName.Trim();
        user.LastName = model.LastName.Trim();
        user.Email = model.Email.ToLower().Trim();
        user.PhoneNumber = model.PhoneNumber?.Trim();

        await _context.SaveChangesAsync();

        await SignInUser(user, user.Role.Name, isPersistent: false);

        TempData["SuccessMessage"] = "Профіль успішно оновлено";
        return RedirectToAction(nameof(Profile));
    }

    // GET: /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    // ─── Хелпери ───────────────────────────────────────────────

    private async Task SignInUser(User user, string roleName, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.LastName} {user.FirstName}".Trim()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, roleName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            ExpiresUtc = isPersistent
                ? DateTimeOffset.UtcNow.AddDays(30)
                : null
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    private static bool VerifyPassword(string inputPassword, string storedHash)
    {
        var inputHash = HashPassword(inputPassword);
        return string.Equals(inputHash, storedHash, StringComparison.OrdinalIgnoreCase);
    }
}