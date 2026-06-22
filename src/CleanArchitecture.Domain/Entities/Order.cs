using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}

public class Order : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public List<OrderItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
}
