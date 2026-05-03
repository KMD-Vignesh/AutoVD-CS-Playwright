using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AutoVD.Framework.Pages
{
    public class CheckoutOverviewPage : BasePage
    {
        private readonly ILocator _finishButton;
        private readonly ILocator _cancelButton;
        private readonly ILocator _totalLabel;
        private readonly ILocator _itemSummary;

        public CheckoutOverviewPage(IPage page) : base(page)
        {
            _finishButton = page.Locator("[data-test='finish']");
            _cancelButton = page.Locator("[data-test='cancel']");
            _totalLabel = page.Locator("[data-test='total-label']");
            _itemSummary = page.Locator(".summary_subtotal_label");
        }

        public async Task<string> GetTotalAsync()
        {
            return await TextContentAsync(_totalLabel);
        }

        public async Task<string> GetSubtotalAsync()
        {
            return await TextContentAsync(_itemSummary);
        }

        public async Task<CheckoutCompletePage> FinishAsync()
        {
            await ClickAsync(_finishButton);
            return new CheckoutCompletePage(Page);
        }

        public async Task<CartPage> CancelAsync()
        {
            await ClickAsync(_cancelButton);
            return new CartPage(Page);
        }
    }
}
