<div align="center">

<h1>AutoVD-CS-Playwright</h1>

<p><strong>Playwright UI test automation framework for .NET</strong><br>
C# · Playwright · NUnit · Page Objects · ExtentReports</p>

<p>
  <a href="https://github.com/KMD-Vignesh/AutoVD-CS-Playwright"><img src="https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10"></a>
  <a href="https://github.com/KMD-Vignesh/AutoVD-CS-Playwright"><img src="https://img.shields.io/badge/Playwright-1.61.0-2EAD33?style=for-the-badge&logo=playwright&logoColor=white" alt="Playwright 1.61.0"></a>
  <a href="https://github.com/KMD-Vignesh/AutoVD-CS-Playwright"><img src="https://img.shields.io/badge/NUnit-4.3.2-1B6EC2?style=for-the-badge" alt="NUnit 4.3.2"></a>
  <a href="https://github.com/KMD-Vignesh/AutoVD-CS-Playwright/blob/main/LICENSE"><img src="https://img.shields.io/badge/License-MIT-2EA44F?style=for-the-badge" alt="MIT License"></a>
</p>

</div>

## Intro

<div>

<img src="https://img.shields.io/badge/Vignesh&nbsp;D,&nbsp;QA&nbsp;Lead&nbsp;/&nbsp;Architect,Chennai,India-B2C6DB?style=for-the-badge" alt="Vignesh">

<p>
    <a href="https://www.linkedin.com/in/kmd-vignesh/"><img src="https://img.shields.io/badge/LinkedIn-0A75AD?style=for-the-badge" alt="LinkedIn"></a>
    <a href="https://github.com/KMD-Vignesh"><img src="https://img.shields.io/badge/GitHub-FF6A80?style=for-the-badge" alt="GitHub"></a>
    <a href="mailto:KMD.Vignesh@outlook.com"><img src="https://img.shields.io/badge/KMD.Vignesh@Outlook-2a9d8f?style=for-the-badge" alt="Email"></a>

</p>

</div>

## What It Is

A Playwright + NUnit framework for browser tests on .NET. A test class passes the browser and category to `BaseTest` through `[TestFixture]`, and each test gets its own Playwright page, Extent entry, and failure screenshot.

- **One fixture, three browsers** — `[TestFixture("Chromium", "Login")]`, Firefox, and WebKit run the same test class.
- **Page objects with a typed driver** — `BasePage` wraps `IPage` and keeps selectors out of the tests.
- **Test data outside the code** — users and products load from `test-data/test-data.json`.
- **Named reports** — every run writes `Reports/ExtentReport_<timestamp>.html` with browser, category, and author.

## Framework Family

Seven implementations of the same layered idea. Pick the stack your team already uses; the concepts and reports stay familiar.

| Framework                       | Stack                         | Covers    | Repository                                                                                  |
| :------------------------------ | :---------------------------- | :-------- | :------------------------------------------------------------------------------------------ |
| **Java + Selenium**             | Java 17 · Selenium 4 · TestNG | UI + REST | [AutoVD-Java-Selenium](https://github.com/KMD-Vignesh/AutoVD-Java-Selenium)                 |
| **Java + Playwright**           | Java 17 · Playwright · TestNG | UI + REST | [AutoVD-Java-Playwright](https://github.com/KMD-Vignesh/AutoVD-Java-Playwright)             |
| **C# + Playwright** — this repo | C# · Playwright · NUnit       | UI + REST | [AutoVD-CS-Playwright](https://github.com/KMD-Vignesh/AutoVD-CS-Playwright)                 |
| **Python + Selenium**           | Python · Selenium · pytest    | UI + REST | [AutoVD-Python-Selenium](https://github.com/KMD-Vignesh/AutoVD-Python-Selenium)             |
| **Python + Playwright**         | Python · Playwright · pytest  | UI + REST | [AutoVD-Python-Playwright](https://github.com/KMD-Vignesh/AutoVD-Python-Playwright)         |
| **TypeScript + Playwright**     | TypeScript · Playwright       | UI + REST | [AutoVD-Typescript-Playwright](https://github.com/KMD-Vignesh/AutoVD-Typescript-Playwright) |
| **TypeScript + Cypress**        | TypeScript · Cypress          | UI (E2E)  | [AutoVD-Typescript-Cypress](https://github.com/KMD-Vignesh/AutoVD-Typescript-Cypress)       |

## Quick Start

**Prerequisites:** .NET SDK 10, PowerShell 7 (`pwsh`), and the Playwright browser binaries.

```bash
git clone https://github.com/KMD-Vignesh/AutoVD-CS-Playwright.git
cd AutoVD-CS-Playwright
dotnet build AutoVD.Framework.slnx
pwsh tests/AutoVD.Tests/bin/Debug/net10.0/playwright.ps1 install
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj
```

The suite runs 8 test methods across 3 browsers: 24 runs in total (5 login and 3 end-to-end scenarios per browser). Headless is the default.

Open the report:

```bash
open Reports/ExtentReport_*.html
```

On Linux use `xdg-open`; on Windows open the newest file in Explorer.

## Configuration

Settings live in `src/AutoVD.Framework/Config/appsettings.json`.

```json
{
  "BaseUrl": "https://www.saucedemo.com",
  "BrowserOptions": ["Chromium", "Firefox", "WebKit"],
  "DefaultTimeout": 30000,
  "Headless": true
}
```

| Key | Default | What it does |
| :--- | :--- | :--- |
| `BaseUrl` | SauceDemo URL | Starting page for every test |
| `BrowserOptions` | Chromium, Firefox, WebKit | Browsers available to the fixtures |
| `DefaultTimeout` | `30000` | Playwright timeout in milliseconds |
| `SlowMo` | `0` | Delay between actions, in milliseconds |
| `Headless` | `true` | Set `false` to watch the run |
| `ScreenshotPath` | `Reports/Screenshots` | Failure screenshot folder |
| `ReportPath` | `Reports` | Extent report folder |
| `RetryCount` | `1` | Retries for flaky actions |

Test data lives in `test-data/test-data.json` and is loaded by `TestDataLoader` into typed models.

## Writing Tests

A fixture supplies the browser and category; the test gets its page object in `[SetUp]`:

```csharp
[TestFixture("Chromium", "Login")]
[TestFixture("Firefox", "Login")]
[TestFixture("WebKit", "Login")]
public class LoginTests : BaseTest
{
    private LoginPage _loginPage;
    private TestDataFile _testData;

    public LoginTests(string browserName, string category) : base(browserName, category) { }

    [SetUp]
    public void InitPages()
    {
        _loginPage = new LoginPage(Driver.Page);
        _testData = TestDataLoader.Load();
    }

    [Test, Author("Vignesh")]
    public async Task SuccessfulLogin_AsStandardUser()
    {
        await _loginPage.NavigateAsync();
        var user = _testData.Users[0];
        var inventoryPage = await _loginPage.LoginAsAsync(user.Username, user.Password);
        Assert.That(await inventoryPage.IsLoadedAsync(), Is.True);
    }
}
```

A page object extends `BasePage` and keeps selectors private:

```csharp
public class LoginPage : BasePage
{
    private readonly string _username = "#user-name";
    private readonly string _password = "#password";
    private readonly string _login = "#login-button";

    public LoginPage(IPage page) : base(page) { }

    public Task NavigateAsync() =>
        Page.GotoAsync(Core.ConfigReader.LoadSettings().BaseUrl);

    public async Task<InventoryPage> LoginAsAsync(string user, string pass)
    {
        await Page.FillAsync(_username, user);
        await Page.FillAsync(_password, pass);
        await Page.ClickAsync(_login);
        return new InventoryPage(Page);
    }
}
```

## Framework Map

Read these in order. Everything else supports them.

| Project / folder | Key classes | What it does |
| :--- | :--- | :--- |
| `tests/AutoVD.Tests` | `BaseTest`, `GlobalSetup` | NUnit lifecycle, Extent entry, screenshot on failure. **Start here.** |
| `tests/AutoVD.Tests/Tests` | `LoginTests`, `E2ETests` | The test fixtures |
| `src/AutoVD.Framework/Core` | `PlaywrightDriver`, `ConfigReader`, `TestDataLoader` | Browser, config, and JSON data loading |
| `src/AutoVD.Framework/Pages` | `BasePage`, `LoginPage`, `InventoryPage`, cart/checkout pages | Page objects |
| `src/AutoVD.Framework/Reports` | `ExtentReportManager` | Report creation and flush |
| `src/AutoVD.Framework/Models` | `AppSettings`, `TestData` | Typed config and data |
| `src/AutoVD.Framework/Config` | `appsettings.json` | Runtime settings |

A few things carry most of the work:

| Item | Use |
| :--- | :--- |
| `Driver.Page` | Playwright `IPage` for the current test |
| `BasePage.Page` | The same page, available inside page objects |
| `_test.Info(...)` | Write a step to the Extent report |
| `TestDataLoader.Load()` | Typed test data from JSON |
| `[Author("...")]` | Author shown in the report |
| `Assert.That(...)` | NUnit assertions |

## How a Run Works

1. NUnit expands each `[TestFixture(browser, category)]` into its own fixture instance.
2. `[SetUp]` in `BaseTest` starts Playwright, navigates to `BaseUrl`, and creates the Extent test.
3. Fixture `[SetUp]` builds the page objects and loads test data.
4. `[TearDown]` records the result and attaches a failure screenshot.
5. `GlobalSetup` flushes the Extent report once at the end of the assembly.

## Project Layout

```text
AutoVD-CS-Playwright/
├── AutoVD.Framework.slnx            # solution
├── src/AutoVD.Framework/            # framework: Core, Pages, Reports, Models, Config
├── tests/AutoVD.Tests/              # BaseTest, GlobalSetup, Tests
├── test-data/test-data.json         # users and products
├── Reports/                         # generated: Extent HTML + screenshots
└── dotnet-tools.json                # local tool manifest
```

`Reports/` and the `bin/`/`obj/` folders are build output.

## Reports

| Output | Location |
| :--- | :--- |
| ExtentReports HTML | `Reports/ExtentReport_<yyyyMMdd_HHmmss>.html` |
| Failure screenshots | `Reports/Screenshots/` |
| NUnit console output | terminal, or `dotnet test --logger trx` |

Each run creates a new timestamped report instead of overwriting the previous one.

## Running a Subset

Use NUnit's `--filter` with `FullyQualifiedName~` for contains matching:

```bash
# One class on one browser
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj \
  --filter "FullyQualifiedName~LoginTests&FullyQualifiedName~Chromium"

# One test on Firefox
dotnet test tests/AutoVD.Tests/AutoVD.Tests.csproj \
  --filter "FullyQualifiedName~CompletePurchaseFlow&FullyQualifiedName~Firefox"
```

`~` means contains, `&` means AND, and `|` means OR.

## Troubleshooting

| Symptom | Fix |
| :--- | :--- |
| `NETSDK1045` or SDK not found | Install the .NET 10 SDK and check `dotnet --version`. |
| `Executable doesn't exist` when a browser starts | Run the `playwright.ps1 install` step after building. |
| `pwsh` not recognised | Install PowerShell 7, or run the script with `powershell` on Windows. |
| Filter matches nothing | `--filter` is name-based; check the fully qualified test and browser names. |
| Report is missing | Look for the newest `Reports/ExtentReport_*.html`; the filename is timestamped. |
| Tests interfere under parallel runs | Fixtures run in parallel; keep test data per test and avoid shared state. |

## License

MIT. See [`LICENSE`](LICENSE).
