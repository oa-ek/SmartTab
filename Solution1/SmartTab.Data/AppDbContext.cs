using Microsoft.EntityFrameworkCore;
using SmartTab.Core;

namespace SmartTab.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductSpecification> ProductSpecifications { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<BuildPart> BuildParts { get; set; } = null!;
    public DbSet<Manufacturer> Manufacturers { get; set; } = null!;
    public DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<CartItemOption> CartItemOptions { get; set; } = null!;
    public DbSet<OrderItemOption> OrderItemOptions { get; set; } = null!;
    public DbSet<InventoryItem> InventoryItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        DataSeeder.SeedData(modelBuilder);

        modelBuilder.Entity<User>()
        .HasOne(u => u.Role)
        .WithMany(r => r.Users)
        .HasForeignKey(u => u.RoleId)
        .OnDelete(DeleteBehavior.Restrict); // Restrict - якщо прив'язані люди, не видалиться.

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductSpecification>()
            .HasOne(ps => ps.Product)
            .WithMany(p => p.Specifications)
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BuildPart>()
            .HasOne(bp => bp.Pc)
            .WithMany(p => p.PcParts)
            .HasForeignKey(bp => bp.PcId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<BuildPart>()
            .HasOne(bp => bp.Component)
            .WithMany(p => p.PartOfPcs)
            .HasForeignKey(bp => bp.ComponentId)
            .OnDelete(DeleteBehavior.Restrict); // поки компонент в збірці, ніт.

        modelBuilder.Entity<CartItemOption>()
            .HasOne(co => co.Component)
            .WithMany()
            .HasForeignKey(co => co.ComponentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItemOption>()
            .HasOne(oo => oo.Component)
            .WithMany()
            .HasForeignKey(oo => oo.ComponentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventoryItem>()
            .HasOne(i => i.Product)
            .WithMany(p => p.InventoryItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}