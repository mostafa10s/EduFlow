using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void DecreaseStock_ReducesQuantity_WhenSufficientStock()
    {
        var product = new Product { Name = "Keyboard", StockQuantity = 10 };

        product.DecreaseStock(3);

        product.StockQuantity.Should().Be(7);
    }

    [Fact]
    public void DecreaseStock_Throws_WhenQuantityIsZero()
    {
        var product = new Product { Name = "Keyboard", StockQuantity = 10 };

        Action act = () => product.DecreaseStock(0);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DecreaseStock_Throws_WhenInsufficientStock()
    {
        var product = new Product { Name = "Keyboard", StockQuantity = 2 };

        Action act = () => product.DecreaseStock(5);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public void IsInStock_ReturnsTrue_WhenQuantityAvailable()
    {
        var product = new Product { Name = "Mouse", StockQuantity = 5 };

        product.IsInStock(3).Should().BeTrue();
        product.IsInStock(5).Should().BeTrue();
    }

    [Fact]
    public void IsInStock_ReturnsFalse_WhenQuantityExceedsStock()
    {
        var product = new Product { Name = "Mouse", StockQuantity = 5 };

        product.IsInStock(6).Should().BeFalse();
    }

    [Fact]
    public void DecreaseStock_SetsUpdatedAt()
    {
        var product = new Product { Name = "Mouse", StockQuantity = 5 };
        var before = product.UpdatedAt;

        product.DecreaseStock(1);

        product.UpdatedAt.Should().NotBe(before);
    }
}
