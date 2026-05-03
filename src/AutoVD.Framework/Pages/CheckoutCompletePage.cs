using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AutoVD.Framework.Pages
{
    public class CheckoutCompletePage : BasePage
    {
        private readonly ILocator _completeHeader;
        private readonly ILocator _backHomeButton;
        private readonly ILocator _ponyExpressImage;

        public CheckoutCompletePage(IPage page) : base(page)
        {
            _completeHeader = page.Locator("[data-test='complete-header']");
            _backHomeButton = page.Locator("[data-test='back-to-products']");
            _ponyExpressImage = page.Locator(".pony_express");
        }

        public async Task<bool> IsCompleteAsync()
        {
            return await IsVisibleAsync(_completeHeader);
        }

        public async Task<string> GetCompleteMessageAsync()
        {
            return await TextContentAsync(_completeHeader);
        }

        public async Task<InventoryPage> BackHomeAsync()
        {
            await ClickAsync(_backHomeButton);
            return new InventoryPage(Page);
        }
    }
}
