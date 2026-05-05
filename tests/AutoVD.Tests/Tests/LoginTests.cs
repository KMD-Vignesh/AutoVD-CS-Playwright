using System.Threading.Tasks;
using NUnit.Framework;
using AutoVD.Framework.Core;
using AutoVD.Framework.Models;
using AutoVD.Framework.Pages;

namespace AutoVD.Tests.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture("Chromium", "Login")]
    [TestFixture("Firefox", "Login")]
    [TestFixture("WebKit", "Login")]
    public class LoginTests : BaseTest
    {
        private LoginPage _loginPage;
        private TestDataFile _testData;

        public LoginTests(string browserName, string category) : base(browserName, category)
        {
        }

        [SetUp]
        public void InitPages()
        {
            _loginPage = new LoginPage(Driver.Page);
            _testData = TestDataLoader.Load();
        }

        [Test, Description("Verify successful login with standard user"), Author("Vignesh")]
        public async Task SuccessfulLogin_AsStandardUser()
        {
            _test.Info("Navigating to login page");
            await _loginPage.NavigateAsync();

            var user = _testData.Users[0];
            _test.Info($"Logging in as {user.Name}");

            var inventoryPage = await _loginPage.LoginAsAsync(user.Username, user.Password);

            _test.Info("Verifying inventory page loaded");
            var isLoaded = await inventoryPage.IsLoadedAsync();
            Assert.That(isLoaded, Is.True, "Inventory page should be visible after successful login");

            _test.Info("Verifying inventory items are displayed");
            var itemCount = await inventoryPage.GetItemCountAsync();
            Assert.That(itemCount, Is.GreaterThan(0), "At least one inventory item should be displayed");
        }

        [Test, Description("Verify login fails with locked out user"), Author("Vicky")]
        public async Task FailedLogin_AsLockedOutUser()
        {
            _test.Info("Navigating to login page");
            await _loginPage.NavigateAsync();

            var user = _testData.Users[1];
            _test.Info($"Attempting login as {user.Name}");

            await _loginPage.EnterUsername(user.Username);
            await _loginPage.EnterPassword(user.Password);
            await _loginPage.ClickLoginAsync();

            _test.Info("Verifying error message is displayed");
            var isErrorVisible = await _loginPage.IsErrorMessageVisibleAsync();
            Assert.That(isErrorVisible, Is.True, "Error message should be displayed for locked out user");

            var errorMessage = await _loginPage.GetErrorMessageAsync();
            _test.Info($"Error message: {errorMessage}");
            Assert.That(errorMessage, Does.Contain("Epic sadface"), "Error message should contain expected text");
        }

        [Test, Description("Verify login fails with invalid credentials"), Author("KMDV")]
        public async Task FailedLogin_AsInvalidUser()
        {
            _test.Info("Navigating to login page");
            await _loginPage.NavigateAsync();

            var user = _testData.Users[3];
            _test.Info($"Attempting login as {user.Name}");

            await _loginPage.EnterUsername(user.Username);
            await _loginPage.EnterPassword(user.Password);
            await _loginPage.ClickLoginAsync();

            _test.Info("Verifying error message is displayed");
            var isErrorVisible = await _loginPage.IsErrorMessageVisibleAsync();
            Assert.That(isErrorVisible, Is.True, "Error message should be displayed for invalid credentials");
        }

        [Test, Description("Verify login fails with empty credentials"), Author("Vignesh")]
        public async Task FailedLogin_WithEmptyCredentials()
        {
            _test.Info("Navigating to login page");
            await _loginPage.NavigateAsync();

            _test.Info("Clicking login without entering credentials");
            await _loginPage.ClickLoginAsync();

            _test.Info("Verifying error message is displayed");
            var isErrorVisible = await _loginPage.IsErrorMessageVisibleAsync();
            Assert.That(isErrorVisible, Is.True, "Error message should be displayed for empty credentials");

            var errorMessage = await _loginPage.GetErrorMessageAsync();
            _test.Info($"Error message: {errorMessage}");
            Assert.That(errorMessage, Does.Contain("Username is required"), "Error should indicate username is required");
        }

        [Test, Description("Intentionally fails to verify failure screenshot capture"), Author("Vignesh")]
        public async Task VerifyFailureScreenshotCapture()
        {
            _test.Info("Navigating to login page");
            await _loginPage.NavigateAsync();

            _test.Info("This test will intentionally fail to demonstrate failure screenshots");

            var user = _testData.Users[0];
            _test.Info($"Logging in as {user.Name}");
            var inventoryPage = await _loginPage.LoginAsAsync(user.Username, user.Password);

            _test.Info("Verifying inventory page loaded");
            var isLoaded = await inventoryPage.IsLoadedAsync();
            Assert.That(isLoaded, Is.True, "Inventory page should be visible after successful login");

            var shouldFail = true;
            Assert.That(shouldFail, Is.False, "Intentional failure to test screenshot capture on failure");
        }
    }
}
