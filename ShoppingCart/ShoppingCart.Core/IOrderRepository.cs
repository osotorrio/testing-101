namespace ShoppingCart.Core;

/// <summary>
/// Repository for saving orders
/// </summary>
public interface IOrderRepository
{
    Task SaveAsync(string customerId, List<CartItem> items, decimal total);
}
