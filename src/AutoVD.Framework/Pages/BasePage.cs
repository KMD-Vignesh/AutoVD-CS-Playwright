using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AutoVD.Framework.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;
        protected readonly int Timeout;

        protected BasePage(IPage page)
        {
            Page = page;
            Timeout = Core.ConfigReader.LoadSettings().DefaultTimeout;
        }

        protected async Task ClickAsync(ILocator locator)
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = Timeout
            });
            await locator.ClickAsync();
        }

        protected async Task FillAsync(ILocator locator, string text)
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = Timeout
            });
            await locator.FillAsync(text);
        }

        protected async Task<string> TextContentAsync(ILocator locator)
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = Timeout
            });
            return await locator.TextContentAsync() ?? string.Empty;
        }

        protected async Task<bool> IsVisibleAsync(ILocator locator)
        {
            return await locator.IsVisibleAsync();
        }

        protected async Task WaitForUrlAsync(string url)
        {
            await Page.WaitForURLAsync(url, new PageWaitForURLOptions
            {
                Timeout = Timeout
            });
        }
    }
}
