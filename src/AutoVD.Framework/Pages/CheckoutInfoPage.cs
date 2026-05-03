using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AutoVD.Framework.Pages
{
    public class CheckoutInfoPage : BasePage
    {
        private readonly ILocator _firstNameInput;
        private readonly ILocator _lastNameInput;
        private readonly ILocator _postalCodeInput;
        private readonly ILocator _continueButton;
        private readonly ILocator _cancelButton;

        public CheckoutInfoPage(IPage page) : base(page)
        {
            _firstNameInput = page.Locator("[data-test='firstName']");
            _lastNameInput = page.Locator("[data-test='lastName']");
            _postalCodeInput = page.Locator("[data-test='postalCode']");
            _continueButton = page.Locator("[data-test='continue']");
            _cancelButton = page.Locator("[data-test='cancel']");
        }

        public async Task EnterFirstNameAsync(string firstName)
        {
            await FillAsync(_firstNameInput, firstName);
        }

        public async Task EnterLastNameAsync(string lastName)
        {
            await FillAsync(_lastNameInput, lastName);
        }

        public async Task EnterPostalCodeAsync(string postalCode)
        {
            await FillAsync(_postalCodeInput, postalCode);
        }

        public async Task<CheckoutOverviewPage> ContinueAsync(string firstName, string lastName, string postalCode)
        {
            await EnterFirstNameAsync(firstName);
            await EnterLastNameAsync(lastName);
            await EnterPostalCodeAsync(postalCode);
            await ClickAsync(_continueButton);
            return new CheckoutOverviewPage(Page);
        }

        public async Task<CartPage> CancelAsync()
        {
            await ClickAsync(_cancelButton);
            return new CartPage(Page);
        }
    }
}
