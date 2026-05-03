using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using AventStack.ExtentReports;
using AutoVD.Framework.Core;
using AutoVD.Framework.Reports;

namespace AutoVD.Tests
{
    [Parallelizable(ParallelScope.Fixtures)]
    public abstract class BaseTest
    {
        protected PlaywrightDriver Driver;
        protected ExtentTest _test;
        private readonly string _browserName;
        private readonly string _category;
        private readonly string _author;

        protected BaseTest(string browserName, string category = "", string author = "")
        {
            _browserName = browserName;
            _category = category;
            _author = author;
        }

        [SetUp]
        public async Task Setup()
        {
            Driver = new PlaywrightDriver(_browserName);
            await Driver.InitializeAsync();

            var testClass = TestContext.CurrentContext.Test.ClassName?.Replace("AutoVD.Tests.Tests.", string.Empty) ?? string.Empty;
            var testName = TestContext.CurrentContext.Test.Name;

            var category = string.IsNullOrEmpty(_category) ? testClass : $"{testClass}, {_category}";

            _test = ExtentReportManager.CreateTest(
                testName,
                _browserName,
                category,
                _author
            );
            _test.Info($"Browser: {_browserName}");
            _test.Info($"Test Class: {testClass}");
        }

        [TearDown]
        public async Task TearDown()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;

            if (status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var screenshotName = $"{TestContext.CurrentContext.Test.Name}_{_browserName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                await Driver.ScreenshotAsync(screenshotName);

                var fullPath = Path.Combine(ConfigReader.LoadSettings().ScreenshotPath, screenshotName);

                if (File.Exists(fullPath))
                {
                    ExtentReportManager.AttachScreenshot(
                        _test,
                        fullPath,
                        $"Failure Screenshot ({_browserName})"
                    );
                }

                var stackTrace = TestContext.CurrentContext.Result.StackTrace;
                var exception = new Exception(TestContext.CurrentContext.Result.Message)
                {
                    HelpLink = stackTrace
                };
                _test.Fail(exception);
            }
            else if (status == NUnit.Framework.Interfaces.TestStatus.Passed)
            {
                _test.Pass("Test Passed");
            }
            else if (status == NUnit.Framework.Interfaces.TestStatus.Skipped)
            {
                _test.Skip($"Test Skipped: {TestContext.CurrentContext.Result.Message}");
            }

            ExtentReportManager.Flush();

            if (Driver != null)
                await Driver.DisposeAsync();
        }
    }
}
