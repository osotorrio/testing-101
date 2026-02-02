using ShoppingCart.Core;

namespace ShoppingCart.UnitTests.Console
{
    public static class CartTest
    {
        public static void RemoveItem_ShouldRemoveItemAndLeaveCartEmpty_WhenCartOnlyHasOneItem()
        {
            // Test Cart RemoveItem method
            var fakeDiscountService = new FakeDiscountService();
            var fakeOrderRepository = new FakeOrderRepository();

            // Create Cart
            Cart cart = new Cart(fakeDiscountService, fakeOrderRepository);

            // Add Item in Cart
            cart.AddItem("P001", "Product 1", 2, 10.0m);

            // Call Remove Item method
            cart.RemoveItem("P001");

            // Assert Items collection, ItemCount, and IsEmpty change
            if (!cart.IsEmpty || cart.ItemCount != 0 || cart.Items.Count != 0)
            {
                throw new Exception("RemoveItem method failed to update cart state correctly.");
            }

            System.Console.WriteLine($"Test was successful - {nameof(RemoveItem_ShouldRemoveItemAndLeaveCartEmpty_WhenCartOnlyHasOneItem)}");
        }
    }

    public class FakeDiscountService : IDiscountService
    {
        public Task<decimal> GetDiscountPercentageAsync(string customerId)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeOrderRepository : IOrderRepository
    {
        public Task SaveAsync(string customerId, List<CartItem> items, decimal total)
        {
            throw new NotImplementedException();
        }
    }
}
