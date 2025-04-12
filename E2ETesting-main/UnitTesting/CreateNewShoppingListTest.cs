using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ListLife.Models;
using Microsoft.AspNetCore.Mvc;
using ListLife.Pages;
using Moq;
using ListLife.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



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




        public class CreateNewShoppingListWithMoqTest
        {
            // Det här testet verifierar att en shoppinglista skapas och kopplas till rätt användare
            // genom att använda en mockad UserManager och en in-memory-databas (mockad ApplicationDbContext).
            [Fact]
            public async Task OnPostAsync_ShouldSaveListWithUserAndRedirect()
            {

                //Arrange
                // Skapa en testanvändare med ID och namn 
                var testUser = new UserList { Id = "user123", ListName = "Jonas" };

                // Skapa en testanvändare med ID och namn (mockad)
                var store = new Mock<IUserStore<UserList>>();

                // mockad UserManager där alla övriga beroenden sätts till null
                // Dessa null representerar t.ex. lösenordshantering, validerare, loggning osv
                var userManagerMock = new Mock<UserManager<UserList>>(
                    store.Object,
                    null, null, null, null, null, null, null, null // Övriga beroenden som inte används i testet
                );

                // Ställ in så att GetUserAsync alltid returnerar vår testanvändare
                userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                               .ReturnsAsync(testUser);

                // fejkad/simulerad databas med in-memory-databas
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                                .UseInMemoryDatabase(databaseName: "Test_ShoppingList_DB")
                                .Options;
                var dbContext = new ApplicationDbContext(options);

                // Skapa instans av sidan vi testar, med mockad databas och userManager
                var pageModel = new CreateNewShoppingList(dbContext, userManagerMock.Object)
                {
                    ShoppingList = new ShoppingList
                    {
                        Title = "Fredagsinköp"
                    },
                    Products = new List<Product>
                {
                    new Product { Name = "Mjölk", Amount = 2, Category = "Fridge" }
                }
                };


                // Act
                // Kör metoden OnPostAsync - dvs användaren klickar på "Skapa shoppinglista"
                var result = await pageModel.OnPostAsync();

                // Kontrollera omdirigering till MyPage
                var redirect = Assert.IsType<RedirectToPageResult>(result);
                Assert.Equal("/MyPage", redirect.PageName);

                // Kontrollera att shoppinglistan sparades korrekt i databasen
                var savedList = dbContext.ShoppingLists.Include(s => s.Products).FirstOrDefault();
                Assert.NotNull(savedList);
                Assert.Equal("Fredagsinköp", savedList.Title);
                Assert.Equal(testUser.Id, savedList.UserId);
                Assert.Single(savedList.Products);
                Assert.Equal("Mjölk", savedList.Products.First().Name);

                // Hämta produkten
                var savedProduct = savedList.Products.First();

                // Kontrollera produktdata
                Assert.Equal("Mjölk", savedProduct.Name);
                Assert.Equal(2, savedProduct.Amount);
                Assert.Equal("Fridge", savedProduct.Category);
                Assert.Equal(savedList.Id, savedProduct.ShoppingListId);
            }



            [Theory]
            [InlineData("Helgshopping", "Ägg", 6, "Fridge items")]
            [InlineData("Veckohandling", "Pasta", 2, "Pantry items")]
            [InlineData("Frysvaror", "Glass", 1, "Freezer items")]
            public async Task OnPostAsync_SavesListWithVariousInput(string title, string productName, int amount, string category)
            {
                // Arrange - Mocka UserManager
                var store = new Mock<IUserStore<UserList>>();
                var userManagerMock = new Mock<UserManager<UserList>>(store.Object, null, null, null, null, null, null, null, null);
                userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                               .ReturnsAsync(new UserList { Id = "user321", ListName = "TestUser" });

                // Skapa in-memory databas
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unik DB per test
                    .Options;
                var dbContext = new ApplicationDbContext(options);

                // Setup PageModel
                var pageModel = new CreateNewShoppingList(dbContext, userManagerMock.Object)
                {
                    ShoppingList = new ShoppingList { Title = title },
                    Products = new List<Product>
                {
                    new Product { Name = productName, Amount = amount, Category = category }
                }
                };

                // Act
                var result = await pageModel.OnPostAsync();

                // Assert
                var redirect = Assert.IsType<RedirectToPageResult>(result);
                Assert.Equal("/MyPage", redirect.PageName);

                var savedList = dbContext.ShoppingLists.Include(s => s.Products).FirstOrDefault();
                Assert.NotNull(savedList);
                Assert.Equal(title, savedList.Title);
                Assert.Single(savedList.Products);
                Assert.Equal(productName, savedList.Products.First().Name);
            }

        }
    }
}

    



