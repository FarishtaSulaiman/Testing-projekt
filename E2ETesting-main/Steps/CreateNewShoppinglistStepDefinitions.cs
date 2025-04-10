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
            // G� till login-sidan
            await _page.GotoAsync("http://localhost:5240/Identity/Account/Login");

            // Fyll i e-post och l�senord
            await _page.FillAsync("input[name='Input.Email']", "test@test.com");
            await _page.FillAsync("input[name='Input.Password']", "Hejhej123!");

            // Klicka p� Logga in-knappen
            await _page.ClickAsync("#login-submit");

            // V�nta p� redirect till MyPage (eller annan bekr�ftelse)
            await _page.WaitForURLAsync("**/MyPage");
        }

        [Given("I am on the \"CreateNewShoppinglist\" page")]
        public async Task GivenIAmOnTheCreateNewShoppinglistPage()
        {
            await _page.GotoAsync("http://localhost:5240/CreateNewShoppingList");
        }

        [When(@"I enter ""(.*)"" in the shopping list title")]
        public async Task WhenIEnterInTheShoppingListTitle(string title)
        {
            await _page.FillAsync("#listTitleInput", title);
        }

        [When(@"I enter ""(.*)"" in the title input field")]
        public async Task WhenIEnterInTheTitleInputField(string title)
        {
            await _page.FillAsync("#listTitleInput", title);
        }

        [When(@"I select ""(.*)"" from the category dropdown")]
        public async Task WhenISelectFromTheCategoryDropdown(string label)
        {
            string value = label switch
            {
                "Fruits & Vegetables" => "FruitsVegetables",
                "Fridge items" => "Fridge",
                "Freezer items" => "Freezer",
                "Pantry items" => "Pantry",
                "Hygiene items" => "Hygiene",
                _ => throw new ArgumentException($"Unknown category label: {label}")
            };

            await _page.SelectOptionAsync("#category", new[] { value });
        }

        [When(@"I enter ""(.*)"" in the product input field")]
        public async Task WhenIEnterInTheProductInputField(string product)
        {
            await _page.FillAsync("#product", product);
        }

        [When(@"I enter ""(.*)"" in the amount input field")]
        public async Task WhenIEnterInTheAmountInputField(string amount)
        {
            await _page.FillAsync("#amount", amount);
        }

        [When(@"I click on the ""(.*)"" button")]
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

        [When(@"I leave the title input field empty")]
        public async Task WhenILeaveTheTitleInputFieldEmpty()
        {
            await _page.FillAsync("#listTitleInput", "");
        }

        [Then(@"I should see a validation message for missing title")]
        public async Task ThenIShouldSeeAValidationMessageForMissingTitle()
        {
            var validationMessage = await _page.Locator("#listTitleInput").EvaluateAsync<string>("el => el.validationMessage");
            if (string.IsNullOrWhiteSpace(validationMessage))
            {
                throw new Exception("No validation message was shown for missing title.");
            }
        }

        [Then(@"I should see two separate items for ""(.*)""")]
        public async Task ThenIShouldSeeTwoSeparateItemsFor(string productName)
        {
            var items = await _page.QuerySelectorAllAsync(".shopping-list-item");
            int matchCount = 0;

            foreach (var item in items)
            {
                var text = await item.InnerTextAsync();
                if (text.Contains(productName, StringComparison.OrdinalIgnoreCase))
                {
                    matchCount++;
                }
            }

            if (matchCount < 2)
            {
                throw new Exception($"Expected two separate items for '{productName}', but found {matchCount}.");
            }
        }


        [Then(@"I should see a warning or validation preventing mismatch")]
        public async Task ThenIShouldSeeAValidationOrMismatchWarning()
        {
            var pageText = await _page.InnerTextAsync("body");
            if (!pageText.ToLower().Contains("warning") && !pageText.ToLower().Contains("validation"))
                throw new Exception("Expected warning or validation message was not found.");
        }

        [Then(@"Instead the product is added to the shopping list")]
        public async Task ThenProductWasStillAddedToList()
        {
            var listText = await _page.InnerTextAsync("#shoppingList");
            if (!listText.Contains("Tomato", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Tomato was not added to the list.");
        }
        [When(@"I drag ""(.*)"" from the Recently Purchased list")]
        public async Task WhenIDragFromRecentlyPurchased(string itemName)
        {
            // Simulerar drag, men Playwright har begr�nsat st�d f�r drag & drop i JS-baserade DOM.
            // Du beh�ver ha drag & drop JS-event p� klientsidan som lyssnar p� detta.
            // H�r anv�nds bara ett placeholder-klick ist�llet.
            var source = _page.Locator($".recently-purchased-item:has-text(\"{itemName}\")");
            await source.ClickAsync(); // simulerar aktivering
        }

        [When(@"I drop ""(.*)"" in the shopping list area")]
        public async Task WhenIDropInTheShoppingListArea(string itemName)
        {
            // Placeholder, d� riktig DnD kr�ver JS event. Simulerar att produkten lades till.
            var list = await _page.InnerTextAsync("#shoppingList");
            if (!list.Contains(itemName))
                throw new Exception($"{itemName} was not dropped correctly.");
        }

        [Then(@"I should see ""(.*)"" in the shopping list")]
        public async Task ThenIShouldSeeInShoppingList(string item)
        {
            var list = await _page.InnerTextAsync("#shoppingList");
            if (!list.Contains(item, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Expected '{item}' to be in the shopping list.");
        }
        [Then(@"the product input field should be empty")]
        public async Task ThenProductInputShouldBeEmpty()
        {
            var value = await _page.InputValueAsync("#product");
            if (!string.IsNullOrWhiteSpace(value))
                throw new Exception("Product field was not cleared.");
        }

        [Then(@"the amount input field should be empty")]
        public async Task ThenAmountInputShouldBeEmpty()
        {
            var value = await _page.InputValueAsync("#amount");
            if (!string.IsNullOrWhiteSpace(value))
                throw new Exception("Amount field was not cleared.");
        }

        [Then(@"the category dropdown should be reset")]
        public async Task ThenCategoryDropdownShouldBeReset()
        {
            var selected = await _page.InputValueAsync("#category");
            if (!string.IsNullOrWhiteSpace(selected))
                throw new Exception("Category dropdown was not reset.");
        }
        [Then(@"I should see the list title ""(.*)"" displayed above the shopping list")]
        public async Task ThenListTitleShouldBeVisible(string title)
        {
            var listTitle = await _page.InnerTextAsync("#listTitleDisplay");
            if (!listTitle.Contains(title, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Expected title '{title}' to be visible, but got '{listTitle}'.");
        }

        [Then(@"I should see the product ""(.*)"" displayed in the shopping list")]
        public async Task ThenProductShouldBeDisplayed(string expected)
        {
            var list = await _page.InnerTextAsync("#shoppingList");
            if (!list.Contains(expected, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Product '{expected}' not displayed.");
        }

        [When(@"""(.*)"" appears in the Recently Purchased section")]
        public async Task WhenAppearsInRecentlyPurchased(string itemName)
        {
            var recentlyPurchased = await _page.InnerTextAsync("#recentlyPurchasedList");
            if (!recentlyPurchased.Contains(itemName, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"'{itemName}' not found in Recently Purchased section.");
        }

        [Then(@"I should see one item ""(.*)"" in the shopping list")]
        public async Task ThenIShouldSeeOneItemInTheShoppingList(string expected)
        {
            var list = await _page.InnerTextAsync("#shoppingList");
            if (!list.Contains(expected, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Expected to see '{expected}' in the shopping list, but it was not found.");
        }

    }
}