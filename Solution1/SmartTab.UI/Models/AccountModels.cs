using System.ComponentModel.DataAnnotations;

namespace SmartTab.UI.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введіть прізвище та ім'я")]
    [Display(Name = "Прізвище та Ім'я")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Введіть email")]
    [EmailAddress(ErrorMessage = "Невірний формат email")]
    [Display(Name = "Електронна пошта")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Невірний формат телефону")]
    [Display(Name = "Номер телефону")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Введіть пароль")]
    [MinLength(6, ErrorMessage = "Пароль має бути не менше 6 символів")]
    [Display(Name = "Пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Підтвердіть пароль")]
    [Compare("Password", ErrorMessage = "Паролі не збігаються")]
    [Display(Name = "Підтвердження паролю")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;

    [Display(Name = "Погоджуюсь з Умовами використання")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Потрібно погодитись з умовами")]
    public bool AgreeToTerms { get; set; }
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Введіть email")]
    [EmailAddress(ErrorMessage = "Невірний формат email")]
    [Display(Name = "Електронна пошта")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Введіть пароль")]
    [Display(Name = "Пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Display(Name = "Запам'ятати мене")]
    public bool RememberMe { get; set; }
}

public class ProfileViewModel
{
    [Required(ErrorMessage = "Введіть прізвище")]
    [Display(Name = "Прізвище")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Введіть ім'я")]
    [Display(Name = "Ім'я")]
    public string FirstName { get; set; } = null!;

    [Phone(ErrorMessage = "Невірний формат телефону")]
    [Display(Name = "Номер телефону")]
    public string? PhoneNumber { get; set; }

    [EmailAddress(ErrorMessage = "Невірний формат email")]
    [Display(Name = "Електронна пошта")]
    public string Email { get; set; } = null!;

    [Display(Name = "Новий пароль (залиште порожнім, якщо не змінюєте)")]
    [MinLength(6, ErrorMessage = "Пароль має бути не менше 6 символів")]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "Паролі не збігаються")]
    [Display(Name = "Підтвердження нового паролю")]
    [DataType(DataType.Password)]
    public string? ConfirmNewPassword { get; set; }

    [Display(Name = "Поточний пароль (для підтвердження змін)")]
    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    public string? RoleName { get; set; }
    public DateTime RegistrationDate { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Введіть email")]
    [EmailAddress(ErrorMessage = "Невірний формат email")]
    [Display(Name = "Електронна пошта")]
    public string Email { get; set; } = null!;
}

public class ResetPasswordViewModel
{
    [Required]
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "Введіть новий пароль")]
    [MinLength(6, ErrorMessage = "Пароль має бути не менше 6 символів")]
    [Display(Name = "Новий пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Підтвердіть пароль")]
    [Compare("Password", ErrorMessage = "Паролі не збігаються")]
    [Display(Name = "Підтвердження паролю")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;
}
