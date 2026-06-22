using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Infrastructure.Persistence;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Products.Any()) return;

        db.Products.AddRange(
            new Product { Name = "Mechanical Keyboard", Description = "TKL, hot-swap, brown switches", Price = 129.99m, StockQuantity = 25 },
            new Product { Name = "Wireless Mouse",      Description = "Ergo, 4K polling",                 Price = 79.50m,  StockQuantity = 40 },
            new Product { Name = "USB-C Hub",           Description = "7-in-1, 100W PD",                  Price = 49.00m,  StockQuantity = 0   });

        db.Customers.AddRange(
            new Customer { FirstName = "Ada",  LastName = "Lovelace", Email = "ada@example.com" },
            new Customer { FirstName = "Alan", LastName = "Turing",   Email = "alan@example.com" });

        db.SaveChanges();
    }
}
