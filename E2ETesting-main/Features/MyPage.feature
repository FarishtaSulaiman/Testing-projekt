#Feature: MyPage
#
#A short summary of the feature
#
#@tag1
#
#Scenario: Open MyPage and view list of shopping lists
#  Given I am logged in as a user
#  When I navigate to "MyPage"
#  Then I should see a heading with text "Your shopping lists"
#  And I should see a list of my existing shopping lists
#
#  Scenario: Click button to create a new shopping list
#  Given I am on the "MyPage" view
#  When I click the button labeled "Create new shopping list"
#  Then I should be redirected to the "CreateNewShoppingList" page
#
#  Scenario: Delete an existing shopping list
#  Given I have at least one shopping list on MyPage
#  When I click the delete button for a shopping list
#  Then the shopping list should be removed from the list
#  And I should stay on the MyPage view
#
#Scenario: Edit an existing shopping list
#  Given I have at least one shopping list on MyPage
#  When I click the edit button for a shopping list
#  Then I should see the heading "Edit shopping list"
#  And I should see the list’s products displayed for editing
#
#  Scenario: Add a new product to a shopping list in edit mode
#  Given I am editing a shopping list
#  When I click the "Add Product" button
#  And I enter "Milk" into the product name field
#  And I enter "1" into the product amount field
#  And I select "Fridge" from the category dropdown
#  Then I click the "Add to Shoppinglist" button
#  And the product "Milk (1) Fridge" should appear in the product list
#
#  Scenario: Share a shopping list with another user
#  Given I have at least one shopping list on MyPage
#  When I click the share button for that list
#  And I enter a valid user email in the email input field
#  When I click the "Send" button
#  Then I should see a success message saying "List shared successfully!"
#
