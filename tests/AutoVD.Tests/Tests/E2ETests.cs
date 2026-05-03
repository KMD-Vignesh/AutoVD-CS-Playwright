using System.Threading.Tasks;
using NUnit.Framework;
using AutoVD.Framework.Core;
using AutoVD.Framework.Models;
using AutoVD.Framework.Pages;

namespace AutoVD.Tests.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture("Chromium", "E2E", "Vignesh")]
    [TestFixture("Firefox", "E2E", "Vignesh")]
    [TestFixture("WebKit", "E2E", "Vignesh")]
    public class E2ETests : BaseTest
    {
        private LoginPage _loginPage;
        private TestDataFile _testData;

        public E2ETests(string browserName, string category, string author) : base(browserName, category, author)
        {
        }

        [SetUp]
        public void InitPages()
        {
            _loginPage = new LoginPage(Driver.Page);
            _testData = TestDataLoader.Load();
        }

        [Test, Description("Complete purchase flow: login, add items, checkout, complete order")]
        public async Task CompletePurchaseFlow()
        {
            var user = _testData.Users[0];

            _test.Info($"Logging in as {user.Name}");
            await _loginPage.NavigateAsync();
            var inventoryPage = await _loginPage.LoginAsAsync(user.Username, user.Password);

            _test.Info("Verifying inventory page loaded");
            Assert.That(await inventoryPage.IsLoadedAsync(), Is.True, "Inventory page should be visible");

            var firstItem = _testData.Products[0];
            var secondItem = _testData.Products[1];

            _test.Info($"Adding '{firstItem.Name}' to cart");
            await inventoryPage.AddItemToCartByNameAsync(firstItem.Name);

            _test.Info($"Adding '{secondItem.Name}' to cart");
            await inventoryPage.AddItemToCartByNameAsync(secondItem.Name);

            var cartCount = await inventoryPage.GetCartBadgeCountAsync();
            _test.Info($"Cart badge shows {cartCount} items");
            Assert.That(cartCount, Is.EqualTo(2), "Cart should have 2 items");

            _test.Info("Navigating to cart");
            var cartPage = await inventoryPage.NavigateToCartAsync();
            Assert.That(await cartPage.IsLoadedAsync(), Is.True, "Cart page should be visible");

            var cartItemCount = await cartPage.GetCartItemCountAsync();
            _test.Info($"Cart contains {cartItemCount} items");
            Assert.That(cartItemCount, Is.EqualTo(2), "Cart should display 2 items");

            _test.Info("Proceeding to checkout");
            var checkoutInfoPage = await cartPage.ClickCheckoutAsync();

            _test.Info("Entering checkout information");
            var checkoutOverviewPage = await checkoutInfoPage.ContinueAsync(
                firstName: "John",
                lastName: "Doe",
                postalCode: "12345"
            );

            _test.Info("Verifying checkout overview");
            var subtotal = await checkoutOverviewPage.GetSubtotalAsync();
            _test.Info($"Subtotal: {subtotal}");

            _test.Info("Completing purchase");
            var checkoutCompletePage = await checkoutOverviewPage.FinishAsync();

            _test.Info("Verifying order completion");
            var isComplete = await checkoutCompletePage.IsCompleteAsync();
            Assert.That(isComplete, Is.True, "Checkout complete page should be displayed");

            var completeMessage = await checkoutCompletePage.GetCompleteMessageAsync();
            _test.Info($"Completion message: {completeMessage}");
            Assert.That(completeMessage, Does.Contain("Thank you"), "Should show complete message");

            _test.Info("Returning to inventory page");
            await checkoutCompletePage.BackHomeAsync();
            Assert.That(await inventoryPage.IsLoadedAsync(), Is.True, "Should return to inventory page");
        }

        [Test, Description("Verify user can logout successfully")]
        public async Task LogoutFlow()
        {
            var user = _testData.Users[0];

            _test.Info($"Logging in as {user.Name}");
            await _loginPage.NavigateAsync();
            await _loginPage.LoginAsAsync(user.Username, user.Password);

            var inventoryPage = new InventoryPage(Driver.Page);
            Assert.That(await inventoryPage.IsLoadedAsync(), Is.True, "Should be on inventory page");

            _test.Info("Logging out");
            await inventoryPage.LogoutAsync();

            _test.Info("Verifying redirected to login page");
            var currentUrl = await _loginPage.GetCurrentUrlAsync();
            _test.Info($"Current URL: {currentUrl}");
            var isLoginPage = await _loginPage.IsErrorMessageVisibleAsync() == false &&
                              (currentUrl.Contains("saucedemo.com") || currentUrl.Contains("login"));
            Assert.That(isLoginPage, Is.True, "Should redirect to login page after logout");
        }

        [Test, Description("Verify all inventory items are displayed")]
        public async Task VerifyInventoryItems()
        {
            var user = _testData.Users[0];

            _test.Info($"Logging in as {user.Name}");
            await _loginPage.NavigateAsync();
            var inventoryPage = await _loginPage.LoginAsAsync(user.Username, user.Password);

            var itemCount = await inventoryPage.GetItemCountAsync();
            _test.Info($"Found {itemCount} inventory items");
            Assert.That(itemCount, Is.EqualTo(6), "Should display 6 inventory items");

            _test.Info("Verifying first product details");
            var itemName = await inventoryPage.GetItemNameAsync(0);
            var itemPrice = await inventoryPage.GetItemPriceAsync(0);

            _test.Info($"First item: {itemName} - {itemPrice}");
            Assert.That(itemName, Is.Not.Null, "Item name should not be null");
            Assert.That(itemPrice, Is.Not.Null, "Item price should not be null");
            Assert.That(itemPrice, Does.StartWith("$"), "Price should start with $");
        }
    }
}
