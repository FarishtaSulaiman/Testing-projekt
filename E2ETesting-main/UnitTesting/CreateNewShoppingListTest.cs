using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ListLife.Models;
using Microsoft.AspNetCore.Mvc;
using ListLife.Pages;


namespace E2ETesting.UnitTesting
{
    public class CreateNewShoppingListTest
    {

        // testar att en shoppinglist kan skapas med en titel
        [Fact]
        public void CanCreateShoppingListWithTitle()
        {
            // Arrange - skapar en shoppinglist med en titel
            var list = new ShoppingList
            {
                Title = "Fredagsinköp"
            };

            // Act & Assert - kontrollerar att titeln är korrekt
            Assert.Equal("Fredagsinköp", list.Title);
        }
        // testar att produkter kan läggas till i en shoppinglista
        [Fact]
        public void CanAddProductsToShoppingList()
        {
            // Arrange - skapar en shoppinglista och två produkter
            var list = new ShoppingList { Title = "Veckohandling" };
            var product1 = new Product { Name = "Mjölk", Amount = 2, Category = "Fridge" };
            var product2 = new Product { Name = "Pasta", Amount = 1, Category = "Pantry" };

            // Act - lägger till produkterna i shoppinglistan
            list.Products.Add(product1);
            list.Products.Add(product2);

            // Assert - kontrollerar att produkterna har lagts till i shoppinglistan
            Assert.Equal(2, list.Products.Count);
            Assert.Contains(product1, list.Products);
            Assert.Contains(product2, list.Products);
        }

        // testar att en shoppinglista kan skapas med en användare
        [Fact]
        public void CanAssignUserToShoppingList()
        {
            // Arrange - skapar en användare och en shoppinglista kopplad till användaren
            var user = new UserList { Id = "12", ListName = "Jonas" };
            var shoppingList = new ShoppingList
            {
                UserId = user.Id,
                UserList = user
            };

            // Assert - kontrollerar att användaren är kopplad till shoppinglistan
            Assert.Equal("12", shoppingList.UserId);
            Assert.Equal("Jonas", shoppingList.UserList.ListName);
        }







        // Denna klass simulerar CreateNewShoppingListsidan utan att använda databas/användarhantering
        // Den används för att testa logiken i OnPostAsync-metoden isolerat
        public class CreateNewShoppingListMock : CreateNewShoppingList
        {
            // Lista som simulerar de sparade shoppinglistorna
            public List<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();

            // Konstruktor som anropar basklassens konstruktor men med null eftersom vi inte behöver verklig databas
            public CreateNewShoppingListMock() : base(null, null) { }

            // Vi överskuggar (override) OnPostAsync för att styra vad som händer vid testning
            public virtual async Task<IActionResult> OnPostAsync()
            {
                // Skapar ny shoppinglista med fallback-titel om titel saknas
                var newShoppingList = new ShoppingList
                {
                    Title = ShoppingList.Title ?? "New Shopping List",
                    Products = Products
                };

                // Sparar listan i vår simulerade lista
                ShoppingLists.Add(newShoppingList);

                // Returnerar redirect till MyPage precis som i originalkoden
                return new RedirectToPageResult("/MyPage");
            }
        }

        // Här är själva testklassen som testar om titel inte anges så används vår "fallback"-titel 
        [Fact]
            public async Task OnPostAsyncWithoutTitleShouldUseFallbackTitle()
            {
                // Arrange - skapar ett mockat PageModel och tilldelar null som titel samt lägger till en produkt
                var pageModel = new CreateNewShoppingListMock
                {
                    ShoppingList = new ShoppingList { Title = null }, // användaren inte fyllt i någon titel
                    Products = new List<Product>
                {
                    new Product { Name = "Apples", Amount = 3, Category = "Fruits & Vegetables" }
                }
                };

                // Act - kör metoden OnPostAsync (som om användaren tryckt på "Create Shopping List")
                var result = await pageModel.OnPostAsync();

            // Assert - kontrollerar att man faktiskt blir omdirigerad till MyPage
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
                Assert.Equal("/MyPage", redirectResult.PageName);

                // Hämtar skapad lista och kontrollerar att den existerar
                var createdList = pageModel.ShoppingLists.FirstOrDefault();
                Assert.NotNull(createdList);

                // Kontrollerar att fallback-titeln har använts när titel saknades
                Assert.Equal("New Shopping List", createdList.Title);
            }
        
    }
}
    



