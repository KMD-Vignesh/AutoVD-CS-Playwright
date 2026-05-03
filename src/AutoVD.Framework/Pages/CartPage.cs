using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AutoVD.Framework.Pages
{
    public class CartPage : BasePage
    {
        private readonly ILocator _cartItems;
        private readonly ILocator _checkoutButton;
        private readonly ILocator _continueShoppingButton;

        public CartPage(IPage page) : base(page)
        {
            _cartItems = page.Locator(".cart_item");
            _checkoutButton = page.Locator("[data-test='checkout']");
            _continueShoppingButton = page.Locator("[data-test='continue-shopping']");
        }

        public async Task<bool> IsLoadedAsync()
        {
            return await IsVisibleAsync(_checkoutButton);
        }

        public async Task<int> GetCartItemCountAsync()
        {
            return await _cartItems.CountAsync();
        }

        public async Task<string> GetItemNameAsync(int index)
        {
            var nameLocator = Page.Locator(".inventory_item_name").Nth(index);
            return await TextContentAsync(nameLocator);
        }

        public async Task<string> GetItemPriceAsync(int index)
        {
            var priceLocator = Page.Locator(".inventory_item_price").Nth(index);
            return await TextContentAsync(priceLocator);
        }

        public async Task<CheckoutInfoPage> ClickCheckoutAsync()
        {
            await ClickAsync(_checkoutButton);
            return new CheckoutInfoPage(Page);
        }

        public async Task<InventoryPage> ContinueShoppingAsync()
        {
            await ClickAsync(_continueShoppingButton);
            return new InventoryPage(Page);
        }
    }
}
