using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTab.Core;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}