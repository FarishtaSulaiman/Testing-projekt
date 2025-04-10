using Xunit;
using ListLife.Models;
namespace E2ETesting.UnitTesting;


public class ProductTest
{
    [Fact]
    public void CanCreateProduct_WithValidProperties()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Mjölk",
            Amount = 2,
            Category = "Fridge",
            ShoppingListId = 5
        };

        // Act & Assert
        Assert.Equal(1, product.Id);
        Assert.Equal("Mjölk", product.Name);
        Assert.Equal(2, product.Amount);
        Assert.Equal("Fridge", product.Category);
        Assert.Equal(5, product.ShoppingListId);
    }

    [Fact]
    public void Product_Default_ShoppingList_ShouldBeNull()
    {
        // Arrange
        var product = new Product();

        // Act & Assert
        Assert.Null(product.ShoppingList);
    }
}
