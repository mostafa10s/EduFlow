using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CleanArchitecture.Tests.Application;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _products;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _products = new Mock<IProductRepository>();
        _uow = new Mock<IUnitOfWork>();
        _uow.Setup(u => u.Products).Returns(_products.Object);
        _sut = new ProductService(_uow.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenProductMissing()
    {
        _products.Setup(p => p.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Product?)null);

        var result = await _sut.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_PersistsAndReturnsDto()
    {
        var dto = new CreateProductDto("Laptop", "Gaming", 1999.99m, 5);

        var result = await _sut.CreateAsync(dto);

        result.Name.Should().Be("Laptop");
        result.Price.Should().Be(1999.99m);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
