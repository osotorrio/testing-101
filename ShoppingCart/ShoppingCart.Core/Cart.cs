namespace ShoppingCart.Core;

/// <summary>
/// Shopping cart - System Under Test (SUT)
/// Demonstrates three types of testing:
/// - Value tests: CalculateTotal() returns a calculated value
/// - State tests: Items collection, ItemCount, IsEmpty properties change
/// - Interaction tests: Calls to IDiscountService and IOrderRepository
/// </summary>
public class Cart
{
    private readonly IDiscountService _discountService;
    private readonly IOrderRepository _orderRepository;
    private readonly List<CartItem> _items = new();

    // STATE: Properties that change when items are added/removed
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public int ItemCount => _items.Sum(i => i.Quantity);
    public bool IsEmpty => _items.Count == 0;

    public Cart(IDiscountService discountService, IOrderRepository orderRepository)
    {
        _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    /// <summary>
    /// Adds an item to the cart
    /// STATE TEST: Verify Items collection, ItemCount, and IsEmpty change
    /// </summary>
    public void AddItem(string productId, string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));
        
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem
            {
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity,
                UnitPrice = unitPrice
            });
        }
    }

    /// <summary>
    /// Removes an item from the cart
    /// STATE TEST: Verify Items collection, ItemCount, and IsEmpty change
    /// </summary>
    public void RemoveItem(string productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }

    /// <summary>
    /// Calculates the total price
    /// VALUE TEST: Verify the returned total is correctly calculated
    /// </summary>
    public decimal CalculateTotal()
    {
        return _items.Sum(item => item.Quantity * item.UnitPrice);
    }

    /// <summary>
    /// Checks out the cart with discount and saves the order
    /// VALUE TEST: Returns final total after discount
    /// INTERACTION TEST: Calls IDiscountService.GetDiscountPercentageAsync() and IOrderRepository.SaveAsync()
    /// </summary>
    public async Task<decimal> CheckoutAsync(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        if (IsEmpty)
            throw new InvalidOperationException("Cannot checkout an empty cart");

        var subtotal = CalculateTotal();

        // INTERACTION: Call external discount service
        var discountPercentage = await _discountService.GetDiscountPercentageAsync(customerId);
        
        var discountAmount = subtotal * discountPercentage;
        var finalTotal = subtotal - discountAmount;

        // INTERACTION: Save order to repository
        await _orderRepository.SaveAsync(customerId, _items.ToList(), finalTotal);

        return finalTotal;
    }
}
