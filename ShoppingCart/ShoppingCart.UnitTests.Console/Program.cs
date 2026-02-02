// See https://aka.ms/new-console-template for more information

using ShoppingCart.UnitTests.Console;

Console.WriteLine("Test running - to please Oscar");
CartTest.RemoveItem_ShouldRemoveItemAndLeaveCartEmpty_WhenCartOnlyHasOneItem();
await CartTest.CheckoutAsync_ShouldCallDiscountService_WhenCheckingOut();
await CartTest.CheckoutAsync_ShouldCallOrderRepository_WhenCheckingOut();
Console.ReadKey();