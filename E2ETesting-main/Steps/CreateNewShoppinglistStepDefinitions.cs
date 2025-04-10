using System;
using SpecFlow;
using TechTalk.SpecFlow;
using Microsoft.Playwright;

namespace E2ETesting.Steps
{
    [Binding]
    public class CreateNewShoppinglistStepDefinitions
    {

        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;

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

        [Given("i am logged in")]
        public async Task GivenIAmLoggedIn()
        {
            // Gå till login-sidan
            await _page.GotoAsync("http://localhost:5240/Identity/Account/Login");

            // Fyll i e-post och lösenord
            await _page.FillAsync("input[name='Input.Email']", "test@test.com");
            await _page.FillAsync("input[name='Input.Password']", "Hejhej123!");

            // Klicka på Logga in-knappen
            await _page.ClickAsync("#login-submit");

            // Vänta på redirect till MyPage (eller annan bekräftelse)
            await _page.WaitForURLAsync("**/MyPage");
        }

        [Given("I am on the \"CreateNewShoppinglist\" page")]
        public async Task GivenIAmOnTheCreateNewShoppinglistPage()
        {
            await _page.GotoAsync("http://localhost:5240/CreateNewShoppingList");
        }

        [When("I enter {string} in the shopping list title")]
        public async Task WhenIEnterInTheShoppingListTitle(string title)
        {
            await _page.FillAsync("#listTitleInput", title);
        }

        [When("I enter {string} in the title input field")]
        public async Task WhenIEnterInTheTitleInputField(string title)
        {
            await _page.FillAsync("#listTitleInput", title);
        }

        [When("I select {string} from the category dropdown")]
        public async Task WhenISelectFromTheCategoryDropdown(string label)
        {
            string value = label switch
            {
                "Fruits & Vegetables" => "FruitsVegetables",
                "Fridge items" => "Fridge",
                "Freezer items" => "Freezer",
                "Pantry items" => "Pantry",
                "Hygiene items" => "Hygiene",
                "Fridge" => "Fridge", // special case
                _ => throw new ArgumentException($"Unknown category label: {label}")
            };

            await _page.SelectOptionAsync("#category", new[] { value });
        }

        [When("I enter {string} in the product input field")]
        public async Task WhenIEnterInTheProductInputField(string product)
        {
            await _page.FillAsync("#product", product);
        }

        [When("I enter {string} in the amount input field")]
        public async Task WhenIEnterInTheAmountInputField(string amount)
        {
            await _page.FillAsync("#amount", amount);
        }

        [When("I click on the \"(.*)\" button")]
        public async Task WhenIClickOnButton(string buttonText)
        {
            string selector = buttonText switch
            {
                "Add to list" => "#addToList",
                "Create Shopping List" => "#createListButton",
                _ => throw new ArgumentException($"Unknown button: {buttonText}")
            };

            await _page.ClickAsync(selector);
        }

        [When("I click on the \"Remove\" button for the item {string}")]
        public async Task WhenIClickOnRemoveButtonForItem(string itemName)
        {
            var itemSelector = $"li:has-text(\"{itemName}\") >> button";
            await _page.ClickAsync(itemSelector);
        }

        [Then("the shopping list should be saved")]
        public async Task ThenTheShoppingListShouldBeSaved()
        {
            await _page.WaitForURLAsync("**/MyPage");
        }

        [Then("I should be redirected to {string}")]
        public async Task ThenIShouldBeRedirectedTo(string page)
        {
            await _page.WaitForURLAsync($"**/{page}");
        }

        [Then("I should not see {string} in the shopping list")]
        public async Task ThenIShouldNotSeeInTheShoppingList(string itemName)
        {
            var text = await _page.InnerTextAsync("#shoppingList");
            if (text.Contains(itemName, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"'{itemName}' was found in the shopping list.");
        }
    }
}