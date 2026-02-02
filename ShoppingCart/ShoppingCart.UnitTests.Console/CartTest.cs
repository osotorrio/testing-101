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

        public static async Task CheckoutAsync_ShouldCallDiscountService_WhenCheckingOut()
        {
            var fakeDiscountService = new FakeDiscountService();
            var fakeOrderRepository = new FakeOrderRepository();

            // Create Cart
            Cart cart = new Cart(fakeDiscountService, fakeOrderRepository);

            // Add Item in Cart
            cart.AddItem("P001", "Product 1", 2, 10.0m);

            var expectedCustomerId = "TestCustomerId";
            await cart.CheckoutAsync(expectedCustomerId);

            // Assert
            if(fakeDiscountService.CustomerId != expectedCustomerId)
            {
                throw new Exception($"Test failure - DiscountService not called with expected customer Id");
            }

            System.Console.WriteLine($"Test was successful - {nameof(CheckoutAsync_ShouldCallDiscountService_WhenCheckingOut)}");
        }

        public static async Task CheckoutAsync_ShouldCallOrderRepository_WhenCheckingOut()
        {
            var fakeDiscountService = new FakeDiscountService();
            var fakeOrderRepository = new FakeOrderRepository();

            // Create Cart
            Cart cart = new Cart(fakeDiscountService, fakeOrderRepository);

            // Add Item in Cart
            cart.AddItem("P001", "Product 1", 2, 10.0m);

            var expectedCustomerId = "TestCustomerId";
            var finalTotal = await cart.CheckoutAsync(expectedCustomerId);

            // Assert
            if (fakeOrderRepository.CustomerId != expectedCustomerId)
            {
                throw new Exception($"Test failure - OrderRepository not called with expected customer Id");
            }

            if (cart.ItemCount != fakeOrderRepository.Items.Count)
            {
                throw new Exception($"Test failure - OrderRepository not called with expected items");
            }

            if (cart.Items.First().ProductId != fakeOrderRepository.Items.First().ProductId)
            {
                throw new Exception("Test failure - OrderRepository not called with expected items");
            }

            if (finalTotal != fakeOrderRepository.Total)
            {
                throw new Exception("Test failure - OrderRepository total is not called with expected total");
            }

            System.Console.WriteLine($"Test was successful - {nameof(CheckoutAsync_ShouldCallOrderRepository_WhenCheckingOut)}");
        }
    }

    public class FakeDiscountService : IDiscountService
    {
        public string CustomerId { get; set; }

        public async Task<decimal> GetDiscountPercentageAsync(string customerId)
        {
            CustomerId = customerId;
            return await Task.FromResult<decimal>(0);
        }
    }

    public class FakeOrderRepository : IOrderRepository
    {
        public string CustomerId { get; set; }
        public List<CartItem> Items { get; set; }
        public decimal Total { get; set; }

        public Task SaveAsync(string customerId, List<CartItem> items, decimal total)
        {
            return Task.CompletedTask;
        }
    }
}
