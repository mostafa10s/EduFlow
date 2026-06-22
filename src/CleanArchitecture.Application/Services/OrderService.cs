using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id, ct);
        if (order is null) return null;

        // hydrate customer
        var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId, ct);

        return new OrderDto(
            order.Id,
            order.CustomerId,
            customer?.FullName ?? "(unknown)",
            order.OrderDate,
            order.Status.ToString(),
            order.Total,
            order.Items.Select(i => new OrderItemDto(
                i.ProductId, i.Quantity, i.UnitPrice, i.Subtotal)).ToList());
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId, ct)
            ?? throw new InvalidOperationException($"Customer {dto.CustomerId} not found.");

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow
        };

        foreach (var item in dto.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId, ct)
                ?? throw new InvalidOperationException($"Product {item.ProductId} not found.");

            // Domain method enforces stock invariant
            product.DecreaseStock(item.Quantity);

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        await _unitOfWork.Orders.AddAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new OrderDto(
            order.Id,
            order.CustomerId,
            customer.FullName,
            order.OrderDate,
            order.Status.ToString(),
            order.Total,
            order.Items.Select(i => new OrderItemDto(
                i.ProductId, i.Quantity, i.UnitPrice, i.Subtotal)).ToList());
    }
}
