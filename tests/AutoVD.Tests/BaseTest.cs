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

        protected BaseTest(string browserName, string category = "")
        {
            _browserName = browserName;
            _category = category;
        }

        private static string ResolveAuthor()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var testClass = TestContext.CurrentContext.Test.ClassName;
            var type = Type.GetType(testClass!);
            var method = type?.GetMethod(testName!);
            var authorAttrs = method?.GetCustomAttributes(typeof(NUnit.Framework.AuthorAttribute), inherit: false) as NUnit.Framework.AuthorAttribute[];
            if (authorAttrs?.Length > 0)
            {
                var values = authorAttrs[0].Properties["Author"];
                return (values as System.Collections.IList)?[0]?.ToString() ?? "Unknown";
            }
            return "Unknown";
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
                ResolveAuthor()
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
                if (Driver != null)
                {
                    try
                    {
                        var screenshotBytes = await Driver.CaptureScreenshotAsync();
                        ExtentReportManager.AttachScreenshotInline(
                            _test,
                            screenshotBytes,
                            $"Failure Screenshot ({_browserName})"
                        );
                    }
                    catch { /* Ignore screenshot errors on failure */ }
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
