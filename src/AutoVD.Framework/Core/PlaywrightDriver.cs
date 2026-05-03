using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using AutoVD.Framework.Models;

namespace AutoVD.Framework.Core
{
    public class PlaywrightDriver : IAsyncDisposable
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private readonly AppSettings _settings;
        private readonly string _browserName;

        public IBrowserContext Context { get; private set; }
        public IPage Page { get; private set; }

        public PlaywrightDriver(string browserName)
        {
            _browserName = browserName;
            _settings = ConfigReader.LoadSettings();
        }

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();

            var headless = ResolveHeadless();
            var slowMo = ResolveSlowMo();

            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = headless,
                SlowMo = slowMo
            };

            _browser = _browserName.ToLower() switch
            {
                "chromium" => await _playwright.Chromium.LaunchAsync(launchOptions),
                "firefox" => await _playwright.Firefox.LaunchAsync(launchOptions),
                "webkit" => await _playwright.Webkit.LaunchAsync(launchOptions),
                _ => throw new ArgumentException($"Unsupported browser: {_browserName}")
            };

            Context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                IgnoreHTTPSErrors = true
            });

            Page = await Context.NewPageAsync();
            await Page.GotoAsync(_settings.BaseUrl, new PageGotoOptions
            {
                Timeout = _settings.DefaultTimeout,
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

            if (headless == false)
            {
                var userAgent = await Page.EvaluateAsync<string>("() => navigator.userAgent");
                Console.WriteLine($"Browser: {_browserName} | UserAgent: {userAgent}");
            }
        }

        private bool ResolveHeadless()
        {
            var envHeadless = Environment.GetEnvironmentVariable("HEADED");
            if (bool.TryParse(envHeadless, out bool isHeaded))
            {
                return !isHeaded;
            }
            return _settings.Headless;
        }

        private int ResolveSlowMo()
        {
            var envSlowMo = Environment.GetEnvironmentVariable("SLOWMO");
            if (int.TryParse(envSlowMo, out int slowMo))
            {
                return slowMo;
            }
            return _settings.SlowMo;
        }

        public async Task ScreenshotAsync(string fileName)
        {
            var directory = _settings.ScreenshotPath;
            Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(directory, fileName);
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = fullPath,
                FullPage = true
            });
        }

        public async ValueTask DisposeAsync()
        {
            if (Context != null)
                await Context.CloseAsync();

            if (_browser != null)
                await _browser.CloseAsync();

            _playwright?.Dispose();
        }
    }
}
