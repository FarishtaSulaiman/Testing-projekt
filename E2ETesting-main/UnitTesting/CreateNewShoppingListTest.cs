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

        [Fact]
        public void CanCreateShoppingListWithTitle()
        {
           
            var list = new ShoppingList
            {
                Title = "Fredagsinköp"
            };

            Assert.Equal("Fredagsinköp", list.Title);
        }



      
        [Fact]
        public void CanAddProductsToShoppingList()
        {
            
            var list = new ShoppingList { Title = "Veckohandling" };
            var product1 = new Product { Name = "Mjölk", Amount = 2, Category = "Fridge" };
            var product2 = new Product { Name = "Pasta", Amount = 1, Category = "Pantry" };

            
            list.Products.Add(product1);
            list.Products.Add(product2);

            
            Assert.Equal(2, list.Products.Count);
            Assert.Contains(product1, list.Products);
            Assert.Contains(product2, list.Products);
        }


        [Fact]
        public void CanAssignUserToShoppingList()
        {
         
            var user = new UserList { Id = "12", ListName = "Jonas" };
            var shoppingList = new ShoppingList
            {
                UserId = user.Id,
                UserList = user
            };

          
            Assert.Equal("12", shoppingList.UserId);
            Assert.Equal("Jonas", shoppingList.UserList.ListName);
        }


       
        public class CreateNewShoppingListMock : CreateNewShoppingList
        {
           
            public List<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();

            
            public CreateNewShoppingListMock() : base(null, null) { }

            
            public virtual async Task<IActionResult> OnPostAsync()
            {
                var newShoppingList = new ShoppingList
                {
                    Title = ShoppingList.Title ?? "New Shopping List",
                    Products = Products
                };

                
                ShoppingLists.Add(newShoppingList);

                return new RedirectToPageResult("/MyPage");
            }
        }

        
        [Fact]
        public async Task OnPostAsyncWithoutTitleShouldUseFallbackTitle()
        {
             
                var pageModel = new CreateNewShoppingListMock
                {

                   ShoppingList = new ShoppingList { Title = null }, 
                   Products = new List<Product>

                   {
                       new Product { Name = "Apples", Amount = 3, Category = "Fruits & Vegetables" }
                   }
                };

                
                var result = await pageModel.OnPostAsync();

              
                var redirectResult = Assert.IsType<RedirectToPageResult>(result);
                Assert.Equal("/MyPage", redirectResult.PageName);

                var createdList = pageModel.ShoppingLists.FirstOrDefault();
                Assert.NotNull(createdList);

              
                Assert.Equal("New Shopping List", createdList.Title);
          
        }




        public class CreateNewShoppingListWithMoqTest
        {
          
            [Fact]
            public async Task OnPostAsync_ShouldSaveListWithUserAndRedirect()
            {

                var testUser = new UserList { Id = "user123", ListName = "Jonas" };

               
                var store = new Mock<IUserStore<UserList>>();

                var userManagerMock = new Mock<UserManager<UserList>>(
                    store.Object,
                    null, null, null, null, null, null, null, null 
                );

               
                userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                               .ReturnsAsync(testUser);

               
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                                .UseInMemoryDatabase(databaseName: "Test_ShoppingList_DB")
                                .Options;
                var dbContext = new ApplicationDbContext(options);

                
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


                
                var result = await pageModel.OnPostAsync();

               
                var redirect = Assert.IsType<RedirectToPageResult>(result);
                Assert.Equal("/MyPage", redirect.PageName);

                
                var savedList = dbContext.ShoppingLists.Include(s => s.Products).FirstOrDefault();
                Assert.NotNull(savedList);
                Assert.Equal("Fredagsinköp", savedList.Title);
                Assert.Equal(testUser.Id, savedList.UserId);
                Assert.Single(savedList.Products);
                Assert.Equal("Mjölk", savedList.Products.First().Name);

                var savedProduct = savedList.Products.First();

               
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
              
                var store = new Mock<IUserStore<UserList>>();
                var userManagerMock = new Mock<UserManager<UserList>>(store.Object, null, null, null, null, null, null, null, null);
                userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                               .ReturnsAsync(new UserList { Id = "user321", ListName = "TestUser" });

               
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                    .Options;
                var dbContext = new ApplicationDbContext(options);

              
                var pageModel = new CreateNewShoppingList(dbContext, userManagerMock.Object)
                {
                    ShoppingList = new ShoppingList { Title = title },
                    Products = new List<Product>
                {
                    new Product { Name = productName, Amount = amount, Category = category }
                }
                };

                
                var result = await pageModel.OnPostAsync();

                
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

    



