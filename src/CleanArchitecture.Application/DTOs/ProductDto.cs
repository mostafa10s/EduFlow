namespace CleanArchitecture.Application.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity);

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity);

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity);
