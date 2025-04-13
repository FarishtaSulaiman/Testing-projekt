using System;
using SpecFlow;
using TechTalk.SpecFlow;
using Microsoft.Playwright;

namespace E2ETesting.Steps
{
    [Binding]
    public class MyPageStepDefinitions
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;
        private int _initialListCount;


        [BeforeScenario]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 1000 });
            _context = await _browser.NewContextAsync();
            _page = await _context.NewPageAsync();
        }

        [AfterScenario]
        public async Task Teardown()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        [Given("I am logged in with current user")]
        public async Task GivenIAmLoggedInWithCurrentUser()
        {
            // Gå till login-sidan
            await _page.GotoAsync("http://localhost:5240/Identity/Account/Login");

            // Fyll i e-post och lösenord
            await _page.FillAsync("input[name='Input.Email']", "test@test.com");
            await _page.FillAsync("input[name='Input.Password']", "Hejhej123!");

            // Klicka på Logga in-knappen
            await _page.ClickAsync("#login-submit");

            // Vänta på redirect till MyPage
            await _page.WaitForURLAsync("**/MyPage");
        }

        [Given("I am on the \"MyPage\" page")]
        public async Task GivenIAmOnTheMyPage()
        {
            await _page.GotoAsync("http://localhost:5240/MyPage");
        }

        [When(@"I navigate to ""MyPage""")]
        public async Task WhenINavigateToMyPage()
        {
         
            await _page.GotoAsync("http://localhost:5240/MyPage");
        }

       

        [When(@"I click the ""Create new shopping list"" button")]
        public async Task WhenIClickTheCreateNewShoppingListButton()
        {
            
            await _page.ClickAsync(".btnNewShoppingList");
        }

        [Then(@"I should be redirected to the ""CreateNewShoppingList"" page")]
        public async Task ThenIShouldBeRedirectedToTheCreateNewShoppingListPage()
        {
         
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var currentUrl = _page.Url;

            if (!currentUrl.ToLower().Contains("/createnewshoppinglist"))
            {
                throw new Exception($"Expected to be redirected to CreateNewShoppingList page, but was on: {currentUrl}");
            }
        }

        [When(@"I have at least one shopping list on MyPage")]
        public async Task WhenIHaveAtLeastOneShoppingListOnMyPage()
        {
            var listItems = _page.Locator(".list-group-item");
            _initialListCount = await listItems.CountAsync();

            if (_initialListCount == 0)
            {
                throw new Exception("No shopping lists found on MyPage.");
            }
        }

        [When(@"I click the delete button for a shopping list")]
        public async Task WhenIClickTheDeleteButtonForAShoppingList()
        {
         
            var deleteButton = _page.Locator(".list-group-item button.btn-danger").First;
            await deleteButton.ClickAsync();

         
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        [Then(@"the shopping list should be removed from the list")]
        public async Task ThenTheShoppingListShouldBeRemovedFromTheList()
        {
            var listItems = _page.Locator(".list-group-item");
            var newCount = await listItems.CountAsync();

            if (newCount >= _initialListCount)
            {
                throw new Exception($"Expected fewer lists after deletion, but count was {newCount}, initial was {_initialListCount}.");
            }
        }

        [Then(@"I should stay on the MyPage view")]
        public async Task ThenIShouldStayOnTheMyPageView()
        {
            var currentUrl = _page.Url;
            if (!currentUrl.ToLower().Contains("/mypage"))
            {
                throw new Exception($"Expected to remain on MyPage, but current URL is: {currentUrl}");
            }
        }




        [When(@"I click the edit button for a shopping list")]
        public async Task WhenIClickTheEditButtonForAShoppingList()
        {
            var editButton = _page.Locator(".list-group-item button.btn-primary").First;
            await editButton.ClickAsync();
            await _page.WaitForSelectorAsync(".edit-section");
        }

        [When(@"I click the ""Add Product"" button")]
        public async Task WhenIClickTheAddProductButton()
        {
            await _page.ClickAsync("button:has-text(\"Add Product\")");
            await _page.WaitForSelectorAsync("#addProductForm");
        }

        [When(@"I enter ""(.*)"" into the product name field")]
        public async Task WhenIEnterIntoTheProductNameField(string productName)
        {
            await _page.FillAsync("#inputProductName", productName);
        }

        [When(@"I enter ""(.*)"" into the product amount field")]
        public async Task WhenIEnterIntoTheProductAmountField(string amount)
        {
            await _page.FillAsync("#inputProductAmount", amount);
        }

        [When(@"I select ""(.*)"" from the MyPage category dropdown")]
        public async Task WhenISelectFromTheMyPageCategoryDropdown(string category)
        {
            await _page.SelectOptionAsync("#inputProductCategory", new[] { category });
        }

        [Then(@"I click the ""Add to Shoppinglist"" button")]
        public async Task ThenIClickTheAddToShoppinglistButton()
        {
            await _page.ClickAsync("#addOrUpdateProduct");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        [Then(@"the product ""(.*)"" should appear in the product list")]
        public async Task ThenTheProductShouldAppearInTheProductList(string expectedProduct)
        {
            var productItems = _page.Locator(".product-item");
            var count = await productItems.CountAsync();

            for (int i = 0; i < count; i++)
            {
                var text = await productItems.Nth(i).InnerTextAsync();
                var simplified = text.Replace(Environment.NewLine, "").Trim();

                if (simplified.Contains(expectedProduct))
                {
                    return; 
                }
            }

            throw new Exception($"Could not find product '{expectedProduct}' in the product list.");
        }


        [Then(@"I should see a list of shopping lists shared with me")]
        public async Task ThenIShouldSeeAListOfShoppingListsSharedWithMe()
        {
            var sharedListSection = _page.Locator("h5 + ul.list-group");

            if (!await sharedListSection.IsVisibleAsync())
            {
                throw new Exception("Shared shopping list section is not visible.");
            }

            var listItems = sharedListSection.Locator("li.list-group-item");
            var count = await listItems.CountAsync();

            if (count == 0)
            {
                throw new Exception("No shared shopping lists were found.");
            }
        }

        [Then(@"I should see the list’s products displayed for editing")]
        public async Task ThenIShouldSeeTheListsProductsDisplayedForEditing()
        {
           
            await _page.WaitForSelectorAsync(".edit-section");

            var productItems = _page.Locator(".edit-section .product-item");
            var count = await productItems.CountAsync();

            if (count == 0)
            {
                throw new Exception("No products were displayed for editing.");
            }
        }

        [Then(@"I should see a heading with text ""(.*)""")]
        public async Task ThenIShouldSeeAHeadingWithText(string expectedText)
        {
            var heading = _page.Locator("h1, h2, h3, h4, h5, h6, p");
            var count = await heading.CountAsync();

            for (int i = 0; i < count; i++)
            {
                var text = await heading.Nth(i).InnerTextAsync();
                if (text.Trim() == expectedText)
                    return;
            }

            throw new Exception($"Expected heading '{expectedText}', but it was not found.");
        }

    }
}


    






    



