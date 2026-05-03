using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AutoVD.Framework.Pages
{
    public class InventoryPage : BasePage
    {
        private readonly ILocator _inventoryItems;
        private readonly ILocator _cartButton;
        private readonly ILocator _menuButton;
        private readonly ILocator _logoutLink;
        private readonly ILocator _sortDropdown;
        private readonly ILocator _addCartButtons;

        public InventoryPage(IPage page) : base(page)
        {
            _inventoryItems = page.Locator(".inventory_item");
            _cartButton = page.Locator("[data-test='shopping-cart-link']");
            _menuButton = page.Locator("#react-burger-menu-btn");
            _logoutLink = page.Locator("[data-test='logout-sidebar-link']");
            _sortDropdown = page.Locator("[data-test='product-sort-container']");
            _addCartButtons = page.Locator("[data-test^='add-to-cart']");
        }

        public async Task<bool> IsLoadedAsync()
        {
            await _inventoryItems.First.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = Timeout
            });
            await _menuButton.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = Timeout
            });
            return true;
        }

        public async Task<int> GetItemCountAsync()
        {
            return await _inventoryItems.CountAsync();
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

        public async Task AddItemToCartAsync(int index)
        {
            var addButton = _addCartButtons.Nth(index);
            await ClickAsync(addButton);
        }

        public async Task AddItemToCartByNameAsync(string itemName)
        {
            var count = await _inventoryItems.CountAsync();
            for (int i = 0; i < count; i++)
            {
                var nameLocator = Page.Locator(".inventory_item_name").Nth(i);
                var name = await nameLocator.TextContentAsync();
                if (name != null && name.Contains(itemName))
                {
                    var addButton = _addCartButtons.Nth(i);
                    await ClickAsync(addButton);
                    return;
                }
            }
            throw new System.Exception($"Item '{itemName}' not found in inventory");
        }

        public async Task<int> GetCartBadgeCountAsync()
        {
            var badge = Page.Locator(".shopping_cart_badge");
            if (await IsVisibleAsync(badge))
            {
                var text = await TextContentAsync(badge);
                return int.Parse(text);
            }
            return 0;
        }

        public async Task<CartPage> NavigateToCartAsync()
        {
            await ClickAsync(_cartButton);
            return new CartPage(Page);
        }

        public async Task OpenMenuAsync()
        {
            await _menuButton.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = Timeout
            });
            await ClickAsync(_menuButton);
        }

        public async Task LogoutAsync()
        {
            await OpenMenuAsync();
            await _logoutLink.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = Timeout
            });
            await ClickAsync(_logoutLink);
        }

        public async Task SortByAsync(string option)
        {
            await _sortDropdown.SelectOptionAsync(new SelectOptionValue { Value = option });
        }
    }
}
