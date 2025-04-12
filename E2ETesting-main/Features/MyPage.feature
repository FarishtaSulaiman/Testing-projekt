Feature: MyPage

Background: 
   Given I am logged in with current user 

Scenario: Open MyPage and view list of shopping lists
  Given I am on the "MyPage" page
  When I navigate to "MyPage"
  Then I should see a heading with text "Your lists:"


Scenario: Click button to create a new shopping list
  Given I am on the "MyPage" page
  When I click the "Create new shopping list" button
  Then I should be redirected to the "CreateNewShoppingList" page

Scenario: Delete an existing shopping list
  Given I am on the "MyPage" page
  When I have at least one shopping list on MyPage
  And I click the delete button for a shopping list
  Then the shopping list should be removed from the list
  And I should stay on the MyPage view

Scenario: Edit an existing shopping list
  Given I am on the "MyPage" page
  When I have at least one shopping list on MyPage
  And I click the edit button for a shopping list
  Then I should see a heading with text "Edit shopping list"
  And I should see the list’s products displayed for editing

Scenario: Add a new product to a shopping list in edit mode
  Given I am on the "MyPage" page
  When I click the edit button for a shopping list
  And I click the "Add Product" button
  And I enter "Milk" into the product name field
  And I enter "1" into the product amount field
  And I select "Fridge" from the MyPage category dropdown
  Then I click the "Add to Shoppinglist" button
  And the product "Milk - 1 Fridge" should appear in the product list


Scenario: Shared shopping lists are shown when available
  Given I am logged in with current user
  And I am on the "MyPage" page
  Then I should see a heading with text "Shared lists"
  And I should see a list of shopping lists shared with me

