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
            new Manufacturer { Id = 1, Name = "Intel" },
            new Manufacturer { Id = 2, Name = "AMD" },
            new Manufacturer { Id = 3, Name = "NVIDIA" },
            new Manufacturer { Id = 4, Name = "ASUS" },
            new Manufacturer { Id = 5, Name = "MSI" },
            new Manufacturer { Id = 6, Name = "Gigabyte" },
            new Manufacturer { Id = 7, Name = "Kingston" },
            new Manufacturer { Id = 8, Name = "Samsung" },
            new Manufacturer { Id = 9, Name = "Corsair" },
            new Manufacturer { Id = 10, Name = "Cooler Master" }
        );

        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Customer" }
        );
    }
}
