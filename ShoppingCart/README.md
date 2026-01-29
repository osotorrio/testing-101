# Shopping Cart - Testing Introduction Project

A simple .NET 10 class library project for demonstrating the three types of unit tests.

## Project Structure

- **Solution**: ShoppingCart.sln
- **Project**: ShoppingCart.Core (Class Library - net10.0)

## System Under Test (SUT)

The main class is `Cart` - a simple shopping cart implementation.

## Three Types of Tests

### 1. Value-Based Tests (Return Values)
Test methods that **return calculated values**:

- `CalculateTotal()` - Returns the sum of all items (quantity × price)
- `CheckoutAsync()` - Returns final total after applying discount

**Example**: Verify that adding items with prices $10 and $20 returns a total of $30.

### 2. State-Based Tests (Object State)
Test that **properties of the Cart object change**:

- `Items` - Collection of cart items
- `ItemCount` - Total quantity of all items
- `IsEmpty` - True when cart has no items, false otherwise

**Example**: 
- Initially `IsEmpty` is true
- After `AddItem()`, `IsEmpty` is false and `ItemCount` increases
- After `RemoveItem()`, state changes accordingly

### 3. Interaction-Based Tests (External Calls)
Test that the SUT **calls external dependencies correctly**:

- `IDiscountService.GetDiscountPercentageAsync()` - Verify it's called with correct customer ID
- `IOrderRepository.SaveAsync()` - Verify order is saved with correct data

**Example**: When `CheckoutAsync()` is called, verify the discount service is invoked and repository save is called.

## Key Components

- **Cart** - The SUT with state and business logic
- **CartItem** - Data model for cart items
- **IDiscountService** - External service interface (for mocking)
- **IOrderRepository** - Database interface (for mocking)

## Build Instructions

```bash
cd ShoppingCart
dotnet build
```

## Summary

This simple example naturally demonstrates all three test types:
- **Value**: Calculations return correct results
- **State**: Object properties change as expected
- **Interaction**: External services are called correctly
