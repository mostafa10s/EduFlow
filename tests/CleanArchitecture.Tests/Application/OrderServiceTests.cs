using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CleanArchitecture.Tests.Application;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRepository<Order>> _orders;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _uow = new Mock<IUnitOfWork>();
        _orders = new Mock<IRepository<Order>>();
        _uow.Setup(u => u.Orders).Returns(_orders.Object);
        _sut = new OrderService(_uow.Object);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenCustomerNotFound()
    {
        _uow.Setup(u => u.Customers.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var dto = new CreateOrderDto(42, new List<CreateOrderItemDto>
        {
            new(ProductId: 1, Quantity: 1)
        });

        Func<Task> act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*Customer 42 not found*");
    }

    [Fact]
    public async Task CreateAsync_DecreasesStock_ForEachItem()
    {
        var customer = new Customer { Id = 1, FirstName = "Ada", LastName = "L", Email = "a@a" };
        var product  = new Product { Id = 1, Name = "Keyboard", Price = 100m, StockQuantity = 10 };

        _uow.Setup(u => u.Customers.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _uow.Setup(u => u.Products.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var dto = new CreateOrderDto(1, new List<CreateOrderItemDto>
        {
            new(ProductId: 1, Quantity: 3)
        });

        var order = await _sut.CreateAsync(dto);

        product.StockQuantity.Should().Be(7);
        order.Total.Should().Be(300m);
        order.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_Returns400Equivalent_WhenStockInsufficient()
    {
        var customer = new Customer { Id = 1, FirstName = "Ada", LastName = "L", Email = "a@a" };
        var product  = new Product { Id = 1, Name = "Mouse", Price = 50m, StockQuantity = 2 };

        _uow.Setup(u => u.Customers.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _uow.Setup(u => u.Products.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var dto = new CreateOrderDto(1, new List<CreateOrderItemDto>
        {
            new(ProductId: 1, Quantity: 5)
        });

        Func<Task> act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*Insufficient stock*");
    }
}
