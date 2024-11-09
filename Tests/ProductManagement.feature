Feature: Product Management
As a user of the Product API
I want to manage products
So that I can create, update, view, and delete products in the system

    Scenario: Add a new product
        Given the product does not already exist
        When I add a new product with name "Laptop", price "999.99", and stock "10"
        Then the product should be added successfully
        And I should see the product with name "Laptop" in the list of products

    Scenario: Retrieve an existing product
        Given a product with name "Laptop" exists
        When I retrieve the product by its ID
        Then the product details should show name "Laptop", price "999.99", and stock "10"

    Scenario: Update an existing product
        Given a product with name "Laptop" exists
        When I update the product to have name "Gaming Laptop" and price "1499.99"
        Then the product details should be updated to name "Gaming Laptop" and price "1499.99"

    Scenario: Delete an existing product
        Given a product with name "Laptop" exists
        When I delete the product by its ID
        Then the product should no longer exist in the system