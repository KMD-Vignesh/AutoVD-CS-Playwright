using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AutoVD.Framework.Pages
{
    public class LoginPage : BasePage
    {
        private readonly ILocator _usernameInput;
        private readonly ILocator _passwordInput;
        private readonly ILocator _loginButton;
        private readonly ILocator _errorMessage;

        public LoginPage(IPage page) : base(page)
        {
            _usernameInput = page.Locator("[data-test='username']");
            _passwordInput = page.Locator("[data-test='password']");
            _loginButton = page.Locator("[data-test='login-button']");
            _errorMessage = page.Locator("[data-test='error']");
        }

        public async Task NavigateAsync()
        {
            await Page.GotoAsync(Core.ConfigReader.LoadSettings().BaseUrl);
        }

        public async Task EnterUsername(string username)
        {
            await FillAsync(_usernameInput, username);
        }

        public async Task EnterPassword(string password)
        {
            await FillAsync(_passwordInput, password);
        }

        public async Task ClickLoginAsync()
        {
            await ClickAsync(_loginButton);
        }

        public async Task<InventoryPage> LoginAsAsync(string username, string password)
        {
            await EnterUsername(username);
            await EnterPassword(password);
            await ClickLoginAsync();
            return new InventoryPage(Page);
        }

        public async Task<string> GetErrorMessageAsync()
        {
            return await TextContentAsync(_errorMessage);
        }

        public async Task<bool> IsErrorMessageVisibleAsync()
        {
            return await IsVisibleAsync(_errorMessage);
        }

        public async Task<string> GetCurrentUrlAsync()
        {
            return Page.Url;
        }
    }
}
