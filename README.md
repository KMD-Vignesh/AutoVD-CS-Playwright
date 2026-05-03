# AutoVD Framework - SauceDemo Playwright Automation

Production-grade C# test automation framework using **Playwright**, **NUnit**, and **Extent Reports** for [SauceDemo](https://www.saucedemo.com).

---

## 📁 Project Structure

```
AutoVD-CS-Playwright/
├── .gitignore                          # Git ignore rules
├── AutoVD.Framework.slnx               # Solution file
├── README.md                           # This file
│
├── src/
│   └── AutoVD.Framework/               # Shared framework library
│       ├── Config/
│       │   └── appsettings.json        # App config (URL, browsers, timeouts, paths)
│       ├── Core/
│       │   ├── ConfigReader.cs         # Loads JSON config into AppSettings
│       │   ├── PlaywrightDriver.cs     # Browser lifecycle (init, screenshot, dispose)
│       │   └── TestDataLoader.cs       # Loads JSON test data (users, products)
│       ├── Models/
│       │   ├── AppSettings.cs          # Config POCO model
│       │   └── TestData.cs             # Test data POCO models (User, Product)
│       ├── Pages/                      # Page Object Model (POM)
│       │   ├── BasePage.cs             # Base class: Click, Fill, WaitFor, TextContent
│       │   ├── LoginPage.cs            # Login page: username, password, login, error
│       │   ├── InventoryPage.cs        # Product listing: items, cart, sort, menu, logout
│       │   ├── CartPage.cs             # Shopping cart: items, checkout, continue shopping
│       │   ├── CheckoutInfoPage.cs     # Checkout form: name, postal code
│       │   ├── CheckoutOverviewPage.cs # Order summary: subtotal, total, finish
│       │   └── CheckoutCompletePage.cs # Order confirmation: message, back home
│       ├── Reports/
│       │   └── ExtentReportManager.cs  # Extent HTML reports with tags, author, screenshots
│       └── AutoVD.Framework.csproj     # Framework project (class library)
│
├── test-data/
│   └── test-data.json                  # Test data: users (valid/invalid), products
│
├── tests/
│   └── AutoVD.Tests/                   # Test project (NUnit)
│       ├── BaseTest.cs                 # Parallel fixture setup/teardown, report attach
│       ├── GlobalSetup.cs              # One-time report initialization
│       ├── nunit.runsettings           # NUnit parallel runner settings
│       ├── Tests/
│       │   ├── LoginTests.cs           # Login scenarios (success, locked, invalid, empty)
│       │   └── E2ETests.cs             # End-to-end flows (purchase, logout, inventory)
│       └── AutoVD.Tests.csproj         # Test project file
│
└── Reports/                            # Generated after test run (created automatically)
    ├── ExtentReport_*.html             # Single HTML report with all test results
    └── Screenshots/                    # Failure screenshots (created on test failure)
```

---

## ⚙️ Configuration

### `src/AutoVD.Framework/Config/appsettings.json`

| Key | Default | Description |
|-----|---------|-------------|
| `BaseUrl` | `https://www.saucedemo.com` | Target application URL |
| `BrowserOptions` | `[Chromium, Firefox, WebKit]` | Browsers for parallel execution |
| `DefaultTimeout` | `30000` | Element wait timeout (ms) |
| `SlowMo` | `0` | Playwright slow motion delay (ms, set >0 for debugging) |
| `Headless` | `true` | Run browser in headless mode (`false` for headed/visible) |
| `ScreenshotPath` | `Reports/Screenshots` | Failure screenshot output directory |
| `ReportPath` | `Reports` | Extent report output directory |
| `RetryCount` | `1` | Retry count for flaky tests |

### `test-data/test-data.json`

Contains test data for users and products:

- **Users**: StandardUser, LockedOutUser, ProblemUser, InvalidUser
- **Products**: Sauce Labs Backpack, Bike Light, Bolt T-Shirt

To add more test data, edit this file and access via `TestDataLoader.GetUser("Name")`.

---

## 🚀 Execution Commands

### Prerequisites

```bash
# Install Playwright browsers (run once)
cd tests/AutoVD.Tests/bin/Debug/net10.0
pwsh playwright.ps1 install chromium
pwsh playwright.ps1 install firefox
pwsh playwright.ps1 install webkit
```

### Build

```bash
dotnet build AutoVD.Framework.slnx
```

---

### One Test Case - One Browser (Contains Match)

The `~` operator means **"contains"** — it matches any test whose fully qualified name contains the given text.

```bash
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~TestName&FullyQualifiedName~Browser"
```

**Template:**
```
--filter "FullyQualifiedName~<partial-test-name>&FullyQualifiedName~<browser-name>"
```

**Examples:**

```bash
# Run "SuccessfulLogin_AsStandardUser" on Chromium only
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~SuccessfulLogin_AsStandardUser&FullyQualifiedName~Chromium"

# Run "CompletePurchaseFlow" on Firefox only
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~CompletePurchaseFlow&FullyQualifiedName~Firefox"

# Run "LogoutFlow" on WebKit only
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~LogoutFlow&FullyQualifiedName~WebKit"

# Partial match - runs ANY test containing "Login" on Chromium
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~Login&FullyQualifiedName~Chromium"

# Partial match - runs ANY test containing "Flow" on Firefox
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~Flow&FullyQualifiedName~Firefox"
```

> `~` = Contains (partial match), `&` = AND, `|` = OR

---

### All Tests - All Browsers (Default: Headless, Parallel)

```bash
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj
```

Runs 21 tests across 3 browsers (Chromium, Firefox, WebKit) in parallel.

---

### All Tests - Single Browser

```bash
# Chromium only
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~Chromium"

# Firefox only
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~Firefox"

# WebKit only
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~WebKit"
```

---

### By Test File / Class

```bash
# All login tests (all browsers)
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~LoginTests"

# All E2E tests (all browsers)
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~E2ETests"
```

---

### By Test Name

```bash
# Specific test (all browsers)
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~SuccessfulLogin_AsStandardUser"

# Specific test, single browser
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~CompletePurchaseFlow&FullyQualifiedName~Chromium"

# Multiple tests
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~LogoutFlow|FullyQualifiedName~VerifyInventoryItems"
```

---

### By Category (Login / E2E)

```bash
# Note: Category is set via TestFixture attribute, filter by class name
# Login category
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~LoginTests"

# E2E category
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~E2ETests"
```

---

### By Browser (All Tests)

```bash
# All tests on Chromium
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~Chromium"

# All tests on Firefox
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~Firefox"

# All tests on WebKit
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~WebKit"
```

---

### Headless vs Headed Mode

Framework supports **3 ways** to toggle headless/headed mode:

#### Method 1: Environment Variable (Recommended - No File Edit)

```bash
# Headed mode (browser visible)
HEADED=true dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~SuccessfulLogin_AsStandardUser&FullyQualifiedName~Chromium"

# Headed + SlowMo (watch step-by-step)
HEADED=true SLOWMO=500 dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~SuccessfulLogin_AsStandardUser&FullyQualifiedName~Chromium"

# Headless mode (explicit)
HEADED=false dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj
```

#### Method 2: Config File (Permanent)

Edit `src/AutoVD.Framework/Config/appsettings.json`:

```json
{
  "Headless": false,
  "SlowMo": 500
}
```

Then run:
```bash
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj
```

#### Method 3: .NET Test Environment Variables

```bash
# Headed mode via DOTNET environment variable
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --environment "HEADED=true" --filter "FullyQualifiedName~Chromium"

# Headed + SlowMo
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --environment "HEADED=true" --environment "SLOWMO=1000" --filter "FullyQualifiedName~Chromium"
```

| Flag | Value | Effect |
|------|-------|--------|
| `HEADED` | `true` | Browser window visible |
| `HEADED` | `false` | Browser runs hidden (default) |
| `SLOWMO` | `0` | No delay (default) |
| `SLOWMO` | `500` | 500ms delay between actions |

---

### With Detailed Output

```bash
# Verbose console output
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --logger "console;verbosity=detailed"

# Normal output
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --logger "console;verbosity=normal"

# Minimal output (pass/fail count only)
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --logger "console;verbosity=minimal"
```

---

### Single Test, Single Browser, No Build

```bash
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj \
  --filter "FullyQualifiedName~SuccessfulLogin_AsStandardUser&FullyQualifiedName~Chromium" \
  --no-build
```

---

### With Custom RunSettings

```bash
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --settings tests/AutoVD.Tests/nunit.runsettings
```

---

## 📊 Reports

Reports are generated in the **project root `Reports/`** directory after every run.

```
Reports/
├── ExtentReport_20260503_191028.html   ← Open in browser
└── Screenshots/                        ← Failure screenshots
```

### Report Features

| Feature | Value |
|---------|-------|
| **Browser Tags** | Chromium, Firefox, WebKit |
| **Categories** | Login, E2E, Regression |
| **Author** | Vignesh (customizable per TestFixture) |
| **Failure Screenshots** | Auto-attached on test failure |
| **Timeline** | Test execution timestamps |

---

## 🧪 Test Summary

| Test Class | Tests | Browsers | Total Runs |
|------------|-------|----------|------------|
| `LoginTests` | 4 | Chromium, Firefox, WebKit | 12 |
| `E2ETests` | 3 | Chromium, Firefox, WebKit | 9 |
| **Total** | **7** | **3** | **21** |

### Test Scenarios

**LoginTests**
- `SuccessfulLogin_AsStandardUser` - Valid login, verify inventory loads
- `FailedLogin_AsLockedOutUser` - Locked user, verify error message
- `FailedLogin_AsInvalidUser` - Wrong credentials, verify error
- `FailedLogin_WithEmptyCredentials` - No input, verify validation

**E2ETests**
- `CompletePurchaseFlow` - Login → Add 2 items → Cart → Checkout → Complete → Back Home
- `LogoutFlow` - Login → Open Menu → Logout → Verify redirect
- `VerifyInventoryItems` - Login → Verify 6 items → Check name & price

---

## 🔧 Adding New Tests

### 1. Add test data (if needed)

Edit `test-data/test-data.json`.

### 2. Create a Page Object (if needed)

```csharp
// src/AutoVD.Framework/Pages/NewPage.cs
public class NewPage : BasePage
{
    public NewPage(IPage page) : base(page) { }

    public async Task DoSomethingAsync()
    {
        await ClickAsync(Page.Locator("#some-button"));
    }
}
```

### 3. Create a Test Class

```csharp
// tests/AutoVD.Tests/Tests/NewTests.cs
[Parallelizable(ParallelScope.Self)]
[TestFixture("Chromium", "Category", "Author")]
[TestFixture("Firefox", "Category", "Author")]
[TestFixture("WebKit", "Category", "Author")]
public class NewTests : BaseTest
{
    public NewTests(string browserName, string category, string author)
        : base(browserName, category, author) { }

    [Test]
    public async Task MyNewTest()
    {
        _test.Info("Step 1");
        // ... assertions
        Assert.That(condition, Is.True);
    }
}
```

### 4. Run

```bash
dotnet build AutoVD.Framework.slnx
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj --filter "FullyQualifiedName~NewTests"
```

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────┐
│                   NUnit Tests                    │
│        (LoginTests.cs, E2ETests.cs)              │
├─────────────────────────────────────────────────┤
│                   BaseTest.cs                    │
│   [SetUp] → Init Driver + ExtentTest             │
│   [TearDown] → Report + Screenshot + Dispose     │
├─────────────────────────────────────────────────┤
│                  Page Objects                    │
│  LoginPage → InventoryPage → CartPage → ...      │
├─────────────────────────────────────────────────┤
│                    Core                          │
│  PlaywrightDriver | ConfigReader | TestDataLoader│
├─────────────────────────────────────────────────┤
│                 Config & Data                    │
│  appsettings.json | test-data.json               │
├─────────────────────────────────────────────────┤
│                    Output                        │
│  Reports/ExtentReport.html | Reports/Screenshots │
└─────────────────────────────────────────────────┘
```

---

## 📦 Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.Playwright` | 1.59.0 | Browser automation |
| `Microsoft.Playwright.NUnit` | 1.59.0 | Playwright + NUnit integration |
| `NUnit` | 4.3.2 | Test framework |
| `NUnit3TestAdapter` | 5.0.0 | Visual Studio / CLI test runner |
| `ExtentReports` | 5.0.4 | HTML reporting with tags/authors |
| `Newtonsoft.Json` | 13.0.4 | JSON serialization |
| `Microsoft.Extensions.Configuration` | 10.0.7 | Configuration management |
| `Microsoft.Extensions.Configuration.Binder` | 10.0.7 | Config to POCO binding |
| `Microsoft.Extensions.Configuration.Json` | 10.0.7 | JSON config file support |

---

## 💡 Quick Reference

| Task | Command |
|------|---------|
| Build | `dotnet build AutoVD.Framework.slnx` |
| All tests | `dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj` |
| One test, one browser | `dotnet test ... --filter "FullyQualifiedName~TestName&FullyQualifiedName~Chromium"` |
| One test, all browsers | `dotnet test ... --filter "FullyQualifiedName~TestName"` |
| All tests, one browser | `dotnet test ... --filter "FullyQualifiedName~Chromium"` |
| Test name contains | `dotnet test ... --filter "FullyQualifiedName~Login"` (runs all with "Login" in name) |
| Multiple tests (OR) | `dotnet test ... --filter "FullyQualifiedName~Login\|FullyQualifiedName~Logout"` |
| Headed mode | `HEADED=true dotnet test ...` |
| Headed + slow motion | `HEADED=true SLOWMO=500 dotnet test ...` |
| View report | Open `Reports/ExtentReport_*.html` in browser |
