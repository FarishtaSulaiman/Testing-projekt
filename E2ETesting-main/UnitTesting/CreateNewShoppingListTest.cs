using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ListLife.Models;


namespace E2ETesting.UnitTesting
{
    public class CreateNewShoppingListTest
    {
        [Fact]
        public void CanCreateShoppingListWithTitle()
        {
            // Arrange
            var list = new ShoppingList
            {
                Title = "Fredagsinköp"
            };

            // Act & Assert
            Assert.Equal("Fredagsinköp", list.Title);
        }
    }
}
