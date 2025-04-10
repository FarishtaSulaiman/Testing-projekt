Feature: CreateNewShoppinglist

A short summary of the feature

   Background: 
   Given i am logged in 


  Scenario:Successfully create a new shopping list
  Given I am on the "CreateNewShoppinglist" page
  When I enter "Monday Shopping" in the shopping list title
  And I select "Fridge items" from the category dropdown
  And I enter "Milk" in the product input field
  And I enter "2" in the amount input field
  And I click on the "Add to list" button
  And I select "Fruits & Vegetables" from the category dropdown
  And I enter "Banana" in the product input field
  And I enter "5" in the amount input field
  And I click on the "Add to list" button
  And I click on the "Create Shopping List" button
  Then the shopping list should be saved
  And I should be redirected to "MyPage"
 


  Scenario: Remove a product from the shopping list
  Given I am on the "CreateNewShoppinglist" page
  When I enter "Test List" in the title input field
  And I select "Fridge items" from the category dropdown
  And I enter "Eggs" in the product input field
  And I enter "1" in the amount input field
  And I click on the "Add to list" button
  And I select "Fridge" from the category dropdown
  And I enter "Juice" in the product input field
  And I enter "1" in the amount input field
  And I click on the "Add to list" button
  When I click on the "Remove" button for the item "Eggs"
  Then I should not see "Eggs" in the shopping list


#  Scenario: Try to create a shopping list without a title
#  Given I am on the "CreateNewShoppinglist" page
#  When I leave the title input field empty
#  And I select "Fridge items" from the category dropdown
#  And I enter "Milk" in the product input field
#  And I enter "1" in the amount input field
#  And I click on the "Add to list" button
#  When I click on the "Create Shopping List" button
#  Then I should see a validation message for missing title
#
#  Scenario: Same product added twice does not merge quantity
#  Given I am on the "CreateNewShoppinglist" page
#  And I enter "Fruit List" in the title input field
#  And I select "Fruits & Vegetables" from the category dropdown
#  And I enter "Banana" in the product input field
#  And I enter "3" in the amount input field
#  And I click on the "Add to list" button
#  And I select "Fruits & Vegetables" from the category dropdown
#  And I enter "Banana" in the product input field
#  And I enter "2" in the amount input field
#  When I click on the "Add to list" button
#  Then I see two separate items for "Banana"
#  But I should see one item "Banana (5)" in the shopping list
#
#
#  Scenario: Adding inappropriate product to category
#  Given I am on the "CreateNewShoppinglist" page
#  And I enter "Friday Shopping" in the title input field
#  And I select "Hygiene" from the category dropdown
#  And I enter "Tomato" in the product input field
#  And I enter "2" in the amount input field
#  When I click on the "Add to list" button
#  Then I should see a warning or validation preventing mismatch
#  But Instead the product is added to the shopping list
#
#
#Scenario: Drag a product from Recently Purchased into the shopping list
#  Given I am on the "CreateNewShoppinglist" page
#  And "Tomato" appears in the Recently Purchased section
#  When I drag "Tomato" from the Recently Purchased list
#  And I drop "Tomato" in the shopping list area
#  Then I should see "Tomato" in the shopping list
#
#  Scenario: Product fields are cleared after adding a product
#  Given I am on the "CreateNewShoppinglist" page
#  And I enter "My Shopping" in the title input field
#  And I select "Pantry items" from the category dropdown
#  And I enter "Pasta" in the product input field
#  And I enter "2" in the amount input field
#  When I click on the "Add to list" button
#  Then the product input field should be empty
#  And the amount input field should be empty
#  And the category dropdown should be reset
#
#
#  Scenario: List title is displayed in real-time when first item is added
#  Given I am on the "CreateNewShoppinglist" page
#  And I enter "Dinner shopping" in the title input field
#  And I select "Freezer items" from the category dropdown
#  And I enter "chicken" in the product input field
#  And I enter "1" in the amount input field
#  When I click on the "Add to list" button
#  Then I should see the list title "Dinner shopping" displayed above the shopping list
#  And I should see the product "Chicken (1)" displayed in the shopping list
#


