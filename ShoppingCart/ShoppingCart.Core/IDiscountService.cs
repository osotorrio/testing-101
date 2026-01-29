namespace ShoppingCart.Core;

/// <summary>
/// External service for calculating discounts
/// </summary>
public interface IDiscountService
{
    Task<decimal> GetDiscountPercentageAsync(string customerId);
}
