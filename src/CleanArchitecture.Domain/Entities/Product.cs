using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    // Domain logic — entity enforces its own invariants
    public bool IsInStock(int requestedQuantity) => StockQuantity >= requestedQuantity;

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (!IsInStock(quantity))
            throw new InvalidOperationException($"Insufficient stock for product '{Name}'. " +
                                               $"Requested {quantity}, available {StockQuantity}.");

        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
