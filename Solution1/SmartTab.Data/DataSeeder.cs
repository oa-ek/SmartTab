using Microsoft.EntityFrameworkCore;
using SmartTab.Core;

namespace SmartTab.Data;

public static class DataSeeder
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Готовий ПК" },
            new Category { Id = 2, Name = "Процесор (CPU)" },
            new Category { Id = 3, Name = "Відеокарта (GPU)" },
            new Category { Id = 4, Name = "Материнська плата" },
            new Category { Id = 5, Name = "Оперативна пам'ять (RAM)" },
            new Category { Id = 6, Name = "Накопичувач (SSD/HDD)" },
            new Category { Id = 7, Name = "Блок живлення" },
            new Category { Id = 8, Name = "Ноутбук" }
        );

        // Seed Manufacturers
        modelBuilder.Entity<Manufacturer>().HasData(
            new Manufacturer { Id = 1, Name = "Intel", Country = "United States" },
            new Manufacturer { Id = 2, Name = "AMD", Country = "United States" },
            new Manufacturer { Id = 3, Name = "NVIDIA", Country = "United States" },
            new Manufacturer { Id = 4, Name = "ASUS", Country = "Taiwan" },
            new Manufacturer { Id = 5, Name = "MSI", Country = "Taiwan" },
            new Manufacturer { Id = 6, Name = "Gigabyte", Country = "Taiwan" },
            new Manufacturer { Id = 7, Name = "Kingston", Country = "United States" },
            new Manufacturer { Id = 8, Name = "Samsung", Country = "South Korea" },
            new Manufacturer { Id = 9, Name = "Corsair", Country = "United States" },
            new Manufacturer { Id = 10, Name = "Cooler Master", Country = "Taiwan" }
        );

        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Customer" }
        );
    }
}
