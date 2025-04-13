using Xunit;
using ListLife.Models;
namespace E2ETesting.UnitTesting;


public class ProductTest
{
    [Fact]
    public void CanCreateProductWithValidProperties()
    {
       
        var product = new Product
        {
            Id = 1,
            Name = "Mjölk",
            Amount = 2,
            Category = "Fridge",
            ShoppingListId = 5
        };

        Assert.Equal(1, product.Id);
        Assert.Equal("Mjölk", product.Name);
        Assert.Equal(2, product.Amount);
        Assert.Equal("Fridge", product.Category);
        Assert.Equal(5, product.ShoppingListId);
    }

    [Fact]
    public void ProductDefaultShoppingListShouldBeNull()
    {
        var product = new Product();

        Assert.Null(product.ShoppingList);
    }
}
