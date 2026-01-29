namespace ShoppingCart.UnitTests;

using FakeItEasy;
using FluentAssertions;
using ShoppingCart.Core;

/// <summary>
/// Comprehensive unit tests for the Cart class
/// Tests three categories of behavior:
/// - Value tests: Return values and calculations
/// - State tests: Object state changes
/// - Interaction tests: External service calls
/// </summary>
public class CartTests
{
    private readonly IDiscountService _discountService;
    private readonly IOrderRepository _orderRepository;
    private readonly Cart _cart;

    public CartTests()
    {
        _discountService = A.Fake<IDiscountService>();
        _orderRepository = A.Fake<IOrderRepository>();
        _cart = new Cart(_discountService, _orderRepository);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullDiscountService_ThrowsArgumentNullException()
    {
        // Act
        var action = () => new Cart(null!, _orderRepository);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("discountService");
    }

    [Fact]
    public void Constructor_WithNullOrderRepository_ThrowsArgumentNullException()
    {
        // Act
        var action = () => new Cart(_discountService, null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("orderRepository");
    }

    [Fact]
    public void Constructor_WithValidDependencies_InitializesEmptyCart()
    {
        // Assert
        _cart.Items.Should().BeEmpty();
        _cart.ItemCount.Should().Be(0);
        _cart.IsEmpty.Should().BeTrue();
    }

    #endregion

    #region AddItem Tests

    [Fact]
    public void AddItem_WithValidParameters_AddsItemToCart()
    {
        // Arrange
        var productId = "PROD001";
        var productName = "Laptop";
        var quantity = 1;
        var unitPrice = 999.99m;

        // Act
        _cart.AddItem(productId, productName, quantity, unitPrice);

        // Assert
        _cart.Items.Should().HaveCount(1);
        _cart.Items[0].ProductId.Should().Be(productId);
        _cart.Items[0].ProductName.Should().Be(productName);
        _cart.Items[0].Quantity.Should().Be(quantity);
        _cart.Items[0].UnitPrice.Should().Be(unitPrice);
    }

    [Fact]
    public void AddItem_WithValidParameters_UpdatesItemCount()
    {
        // Arrange
        var quantity = 5;

        // Act
        _cart.AddItem("PROD001", "Mouse", quantity, 29.99m);

        // Assert
        _cart.ItemCount.Should().Be(quantity);
    }

    [Fact]
    public void AddItem_WithValidParameters_UpdatesIsEmptyProperty()
    {
        // Arrange
        var initialIsEmpty = _cart.IsEmpty;

        // Act
        _cart.AddItem("PROD001", "Mouse", 1, 29.99m);
        var afterAddIsEmpty = _cart.IsEmpty;

        // Assert
        initialIsEmpty.Should().BeTrue();
        afterAddIsEmpty.Should().BeFalse();
    }

    [Fact]
    public void AddItem_WithExistingProductId_IncreasesQuantityOfExistingItem()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 2, 29.99m);

        // Act
        _cart.AddItem("PROD001", "Mouse", 3, 29.99m);

        // Assert
        _cart.Items.Should().HaveCount(1);
        _cart.Items[0].Quantity.Should().Be(5);
        _cart.ItemCount.Should().Be(5);
    }

    [Fact]
    public void AddItem_WithMultipleDifferentProducts_AddsMultipleItems()
    {
        // Act
        _cart.AddItem("PROD001", "Mouse", 1, 29.99m);
        _cart.AddItem("PROD002", "Keyboard", 1, 79.99m);
        _cart.AddItem("PROD003", "Monitor", 1, 299.99m);

        // Assert
        _cart.Items.Should().HaveCount(3);
        _cart.ItemCount.Should().Be(3);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddItem_WithNullOrWhiteSpaceProductId_ThrowsArgumentException(string productId)
    {
        // Act
        var action = () => _cart.AddItem(productId, "Product", 1, 10m);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("productId")
            .WithMessage("Product ID cannot be empty*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void AddItem_WithInvalidQuantity_ThrowsArgumentException(int quantity)
    {
        // Act
        var action = () => _cart.AddItem("PROD001", "Product", quantity, 10m);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("quantity")
            .WithMessage("Quantity must be greater than zero*");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void AddItem_WithNegativeUnitPrice_ThrowsArgumentException(decimal unitPrice)
    {
        // Act
        var action = () => _cart.AddItem("PROD001", "Product", 1, unitPrice);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("unitPrice")
            .WithMessage("Unit price cannot be negative*");
    }

    [Fact]
    public void AddItem_WithZeroUnitPrice_Succeeds()
    {
        // Act
        _cart.AddItem("PROD001", "FreeItem", 1, 0m);

        // Assert
        _cart.Items.Should().HaveCount(1);
        _cart.Items[0].UnitPrice.Should().Be(0m);
    }

    #endregion

    #region RemoveItem Tests

    [Fact]
    public void RemoveItem_WithExistingProductId_RemovesItemFromCart()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 1, 29.99m);
        _cart.AddItem("PROD002", "Keyboard", 1, 79.99m);

        // Act
        _cart.RemoveItem("PROD001");

        // Assert
        _cart.Items.Should().HaveCount(1);
        _cart.Items[0].ProductId.Should().Be("PROD002");
    }

    [Fact]
    public void RemoveItem_WithExistingProductId_UpdatesItemCount()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 5, 29.99m);

        // Act
        _cart.RemoveItem("PROD001");

        // Assert
        _cart.ItemCount.Should().Be(0);
    }

    [Fact]
    public void RemoveItem_WithExistingProductId_UpdatesIsEmptyProperty()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 1, 29.99m);

        // Act
        _cart.RemoveItem("PROD001");

        // Assert
        _cart.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void RemoveItem_WithNonExistentProductId_DoesNotThrowException()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 1, 29.99m);

        // Act
        var action = () => _cart.RemoveItem("PROD999");

        // Assert
        action.Should().NotThrow();
        _cart.Items.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveItem_WithNonExistentProductId_DoesNotChangeCart()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 1, 29.99m);
        var itemCountBefore = _cart.ItemCount;

        // Act
        _cart.RemoveItem("PROD999");

        // Assert
        _cart.ItemCount.Should().Be(itemCountBefore);
        _cart.Items.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveItem_RemovingAllItems_CartIsEmpty()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 1, 29.99m);
        _cart.AddItem("PROD002", "Keyboard", 1, 79.99m);

        // Act
        _cart.RemoveItem("PROD001");
        _cart.RemoveItem("PROD002");

        // Assert
        _cart.IsEmpty.Should().BeTrue();
        _cart.Items.Should().BeEmpty();
        _cart.ItemCount.Should().Be(0);
    }

    #endregion

    #region CalculateTotal Tests

    [Fact]
    public void CalculateTotal_WithEmptyCart_ReturnsZero()
    {
        // Act
        var total = _cart.CalculateTotal();

        // Assert
        total.Should().Be(0m);
    }

    [Fact]
    public void CalculateTotal_WithSingleItem_ReturnsCorrectTotal()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 2, 29.99m);

        // Act
        var total = _cart.CalculateTotal();

        // Assert
        total.Should().Be(59.98m);
    }

    [Fact]
    public void CalculateTotal_WithMultipleItems_ReturnsCorrectTotal()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 2, 29.99m);      // 59.98
        _cart.AddItem("PROD002", "Keyboard", 1, 79.99m);   // 79.99
        _cart.AddItem("PROD003", "Monitor", 1, 299.99m);   // 299.99
        // Total: 439.96

        // Act
        var total = _cart.CalculateTotal();

        // Assert
        total.Should().Be(439.96m);
    }

    [Fact]
    public void CalculateTotal_WithZeroPriceItem_IncludesZeroPriceInCalculation()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 1, 29.99m);
        _cart.AddItem("PROD002", "FreeItem", 5, 0m);

        // Act
        var total = _cart.CalculateTotal();

        // Assert
        total.Should().Be(29.99m);
    }

    [Fact]
    public void CalculateTotal_WithHighQuantities_CalculatesCorrectly()
    {
        // Arrange
        _cart.AddItem("PROD001", "Item", 1000, 10.50m);

        // Act
        var total = _cart.CalculateTotal();

        // Assert
        total.Should().Be(10500m);
    }

    [Fact]
    public void CalculateTotal_AfterAddingItem_ReflectsNewTotal()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 1, 29.99m);
        var totalBefore = _cart.CalculateTotal();

        // Act
        _cart.AddItem("PROD002", "Keyboard", 1, 79.99m);
        var totalAfter = _cart.CalculateTotal();

        // Assert
        totalBefore.Should().Be(29.99m);
        totalAfter.Should().Be(109.98m);
    }

    [Fact]
    public void CalculateTotal_AfterRemovingItem_ReflectsNewTotal()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 1, 29.99m);
        _cart.AddItem("PROD002", "Keyboard", 1, 79.99m);
        var totalBefore = _cart.CalculateTotal();

        // Act
        _cart.RemoveItem("PROD001");
        var totalAfter = _cart.CalculateTotal();

        // Assert
        totalBefore.Should().Be(109.98m);
        totalAfter.Should().Be(79.99m);
    }

    #endregion

    #region CheckoutAsync Tests

    [Fact]
    public async Task CheckoutAsync_WithValidCustomerAndItems_ReturnsCorrectFinalTotal()
    {
        // Arrange
        var customerId = "CUST001";
        var discountPercentage = 0.1m; // 10% discount
        
        _cart.AddItem("PROD001", "Mouse", 1, 100m);
        A.CallTo(() => _discountService.GetDiscountPercentageAsync(customerId))
            .Returns(discountPercentage);

        // Act
        var finalTotal = await _cart.CheckoutAsync(customerId);

        // Assert
        // Subtotal: 100m, Discount: 10m, Final: 90m
        finalTotal.Should().Be(90m);
    }

    [Fact]
    public async Task CheckoutAsync_WithNoDiscount_ReturnsSubtotal()
    {
        // Arrange
        var customerId = "CUST001";
        var discountPercentage = 0m; // No discount

        _cart.AddItem("PROD001", "Mouse", 1, 100m);
        A.CallTo(() => _discountService.GetDiscountPercentageAsync(customerId))
            .Returns(discountPercentage);

        // Act
        var finalTotal = await _cart.CheckoutAsync(customerId);

        // Assert
        finalTotal.Should().Be(100m);
    }

    [Fact]
    public async Task CheckoutAsync_WithFullDiscount_ReturnsZero()
    {
        // Arrange
        var customerId = "CUST001";
        var discountPercentage = 1m; // 100% discount

        _cart.AddItem("PROD001", "Mouse", 1, 100m);
        A.CallTo(() => _discountService.GetDiscountPercentageAsync(customerId))
            .Returns(discountPercentage);

        // Act
        var finalTotal = await _cart.CheckoutAsync(customerId);

        // Assert
        finalTotal.Should().Be(0m);
    }

    [Fact]
    public async Task CheckoutAsync_WithMultipleItems_AppliesDiscountToSubtotal()
    {
        // Arrange
        var customerId = "CUST001";
        var discountPercentage = 0.15m; // 15% discount

        _cart.AddItem("PROD001", "Mouse", 2, 50m);     // 100m
        _cart.AddItem("PROD002", "Keyboard", 1, 200m); // 200m
        // Subtotal: 300m, Discount: 45m, Final: 255m

        A.CallTo(() => _discountService.GetDiscountPercentageAsync(customerId))
            .Returns(discountPercentage);

        // Act
        var finalTotal = await _cart.CheckoutAsync(customerId);

        // Assert
        finalTotal.Should().Be(255m);
    }

    [Fact]
    public async Task CheckoutAsync_CallsDiscountServiceWithCorrectCustomerId()
    {
        // Arrange
        var customerId = "CUST001";
        _cart.AddItem("PROD001", "Mouse", 1, 100m);
        A.CallTo(() => _discountService.GetDiscountPercentageAsync(A<string>._))
            .Returns(0m);

        // Act
        await _cart.CheckoutAsync(customerId);

        // Assert
        A.CallTo(() => _discountService.GetDiscountPercentageAsync(customerId))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CheckoutAsync_CallsRepositoryWithCorrectData()
    {
        // Arrange
        var customerId = "CUST001";
        var discountPercentage = 0.1m;

        _cart.AddItem("PROD001", "Mouse", 2, 50m);
        _cart.AddItem("PROD002", "Keyboard", 1, 200m);

        A.CallTo(() => _discountService.GetDiscountPercentageAsync(customerId))
            .Returns(discountPercentage);

        // Act
        await _cart.CheckoutAsync(customerId);

        // Assert - Subtotal 300m, Final 270m
        A.CallTo(() => _orderRepository.SaveAsync(
            customerId,
            A<List<CartItem>>.That.Matches(items =>
                items.Count == 2 &&
                items[0].ProductId == "PROD001" &&
                items[1].ProductId == "PROD002"),
            270m))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CheckoutAsync_WithEmptyCart_ThrowsInvalidOperationException()
    {
        // Act
        var action = () => _cart.CheckoutAsync("CUST001");

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot checkout an empty cart");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CheckoutAsync_WithNullOrWhiteSpaceCustomerId_ThrowsArgumentException(string customerId)
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 1, 100m);

        // Act
        var action = () => _cart.CheckoutAsync(customerId);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId")
            .WithMessage("Customer ID cannot be empty*");
    }

    [Fact]
    public async Task CheckoutAsync_WithValidData_RepositoryIsCalled()
    {
        // Arrange
        var customerId = "CUST001";
        _cart.AddItem("PROD001", "Mouse", 1, 100m);
        A.CallTo(() => _discountService.GetDiscountPercentageAsync(A<string>._))
            .Returns(0m);

        // Act
        await _cart.CheckoutAsync(customerId);

        // Assert
        A.CallTo(() => _orderRepository.SaveAsync(A<string>._, A<List<CartItem>>._, A<decimal>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CheckoutAsync_WithValidData_DiscountServiceIsCalled()
    {
        // Arrange
        var customerId = "CUST001";
        _cart.AddItem("PROD001", "Mouse", 1, 100m);
        A.CallTo(() => _discountService.GetDiscountPercentageAsync(A<string>._))
            .Returns(0m);

        // Act
        await _cart.CheckoutAsync(customerId);

        // Assert
        A.CallTo(() => _discountService.GetDiscountPercentageAsync(A<string>._))
            .MustHaveHappenedOnceExactly();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void FullShoppingFlow_AddMultipleItems_CorrectState()
    {
        // Act
        _cart.AddItem("PROD001", "Mouse", 2, 29.99m);
        _cart.AddItem("PROD002", "Keyboard", 1, 79.99m);
        _cart.AddItem("PROD003", "Monitor", 1, 299.99m);

        // Assert
        _cart.IsEmpty.Should().BeFalse();
        _cart.ItemCount.Should().Be(4);
        _cart.Items.Should().HaveCount(3);
        _cart.CalculateTotal().Should().Be(439.96m);
    }

    [Fact]
    public void FullShoppingFlow_ModifyCart_CorrectState()
    {
        // Arrange
        _cart.AddItem("PROD001", "Mouse", 2, 29.99m);
        _cart.AddItem("PROD002", "Keyboard", 1, 79.99m);

        // Act
        _cart.AddItem("PROD001", "Mouse", 3, 29.99m); // Increase quantity of existing item
        _cart.RemoveItem("PROD002");

        // Assert
        _cart.ItemCount.Should().Be(5);
        _cart.Items.Should().HaveCount(1);
        _cart.CalculateTotal().Should().Be(149.95m);
    }

    [Fact]
    public async Task FullShoppingFlow_AddItemsAndCheckout_Success()
    {
        // Arrange
        var customerId = "CUST001";
        var discountPercentage = 0.2m; // 20% discount

        _cart.AddItem("PROD001", "Mouse", 1, 100m);
        _cart.AddItem("PROD002", "Keyboard", 1, 200m);

        A.CallTo(() => _discountService.GetDiscountPercentageAsync(customerId))
            .Returns(discountPercentage);

        // Act
        var finalTotal = await _cart.CheckoutAsync(customerId);

        // Assert
        // Subtotal: 300m, Discount: 60m, Final: 240m
        finalTotal.Should().Be(240m);
        A.CallTo(() => _discountService.GetDiscountPercentageAsync(customerId))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _orderRepository.SaveAsync(A<string>._, A<List<CartItem>>._, A<decimal>._))
            .MustHaveHappenedOnceExactly();
    }

    #endregion
}
