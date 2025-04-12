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
  And I select "Fridge items" from the category dropdown
  And I enter "Juice" in the product input field
  And I enter "1" in the amount input field
  And I click on the "Add to list" button
  When I click on the "Remove" button for the item "Eggs"
  Then I should not see "Eggs" in the shopping list


  Scenario: Try to create a shopping list without a title
  Given I am on the "CreateNewShoppinglist" page
  When I leave the title input field empty
  And I select "Fridge items" from the category dropdown
  And I enter "Milk" in the product input field
  And I enter "1" in the amount input field
  And I click on the "Add to list" button
  When I click on the "Create Shopping List" button
 #// Then I should see a validation message for missing title

  Scenario: Same product added twice does not merge quantity
  Given I am on the "CreateNewShoppinglist" page
  When I enter "Fruit List" in the title input field
  And I select "Fruits & Vegetables" from the category dropdown
  And I enter "Banana" in the product input field
  And I enter "3" in the amount input field
  And I click on the "Add to list" button
  And I select "Fruits & Vegetables" from the category dropdown
  And I enter "Banana" in the product input field
  And I enter "2" in the amount input field
  When I click on the "Add to list" button
  Then I should see two separate items for "Banana"


  Scenario: Adding inappropriate product to category
  Given I am on the "CreateNewShoppinglist" page
  When I enter "Friday Shopping" in the title input field
  And I select "Hygiene items" from the category dropdown
  And I enter "Tomato" in the product input field
  And I enter "2" in the amount input field
  When I click on the "Add to list" button
  Then I should see "Tomato" in the shopping list
  But I should see a warning or validation preventing mismatch



  Scenario: Product fields are cleared after adding a product
  Given I am on the "CreateNewShoppinglist" page
  When I enter "My Shopping" in the title input field
  And I select "Pantry items" from the category dropdown
  And I enter "Pasta" in the product input field
  And I enter "2" in the amount input field
  And I click on the "Add to list" button
  Then the product input field should be empty
  And the amount input field should be empty
  And the category dropdown should be reset

Scenario: Drag a product from Recently Purchased into the shopping list
  Given I am on the "CreateNewShoppinglist" page
  When I enter "Fredagsinköp" in the title input field
  And I select "Fridge items" from the category dropdown
  And I enter "Mjölk" in the product input field
  And I enter "2" in the amount input field
  And I click on the "Add to list" button
  And "Banana" appears in the Recently Purchased section
  And I drag "Banana" from the Recently Purchased list
  And I drop "Banana" in the shopping list area
  Then I should see "Banana" in the shopping list







