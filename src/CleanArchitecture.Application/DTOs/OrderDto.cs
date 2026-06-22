namespace CleanArchitecture.Application.DTOs;

public record OrderItemDto(int ProductId, int Quantity, decimal UnitPrice, decimal Subtotal);

public record OrderDto(
    int Id,
    int CustomerId,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    decimal Total,
    List<OrderItemDto> Items);

public record CreateOrderDto(
    int CustomerId,
    List<CreateOrderItemDto> Items);

public record CreateOrderItemDto(int ProductId, int Quantity);
