using System;
using System.Collections.Generic;
using System.IO;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace AutoVD.Framework.Reports
{
    public static class ExtentReportManager
    {
        private static readonly object _lock = new();
        private static ExtentReports? _extentReports;
        private static ExtentSparkReporter? _sparkReporter;

        public static ExtentReports Instance
        {
            get
            {
                if (_extentReports == null)
                {
                    lock (_lock)
                    {
                        if (_extentReports == null)
                        {
                            Initialize();
                        }
                    }
                }
                return _extentReports!;
            }
        }

        private static void Initialize()
        {
            var settings = Core.ConfigReader.LoadSettings();
            var reportPath = settings.ReportPath;
            Directory.CreateDirectory(reportPath);

            var reportFile = Path.Combine(reportPath, $"ExtentReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");

            _sparkReporter = new ExtentSparkReporter(reportFile);
            _sparkReporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Standard;
            _sparkReporter.Config.DocumentTitle = "AutoVD Test Report";
            _sparkReporter.Config.ReportName = "SauceDemo Automation Report";
            _sparkReporter.Config.Encoding = "utf-8";
            _sparkReporter.Config.TimeStampFormat = "dd-MM-yyyy HH:mm:ss";

            _extentReports = new ExtentReports();
            _extentReports.AttachReporter(_sparkReporter);

            _extentReports.AddSystemInfo("Framework", "Playwright + C# + NUnit");
            _extentReports.AddSystemInfo("Target Application", "SauceDemo");
            _extentReports.AddSystemInfo("Browsers", "Chromium, Firefox, WebKit");
            _extentReports.AddSystemInfo("OS", Environment.OSVersion.ToString());
            _extentReports.AddSystemInfo("Execution Time", DateTime.Now.ToString());
        }

        public static ExtentTest CreateTest(string testName, string browserName, string category = "", string author = "")
        {
            lock (_lock)
            {
                var test = Instance.CreateTest(testName);

                test.AssignAuthor(string.IsNullOrEmpty(author) ? "AutoVD Framework" : author);

                test.AssignDevice(browserName);

                if (!string.IsNullOrEmpty(category))
                {
                    foreach (var cat in category.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        test.AssignCategory(cat.Trim());
                    }
                }

                test.AssignCategory("Regression");

                return test;
            }
        }

        public static void Flush()
        {
            lock (_lock)
            {
                _extentReports?.Flush();
            }
        }

        public static void Close()
        {
            lock (_lock)
            {
                _extentReports?.Flush();
            }
        }

        public static void AttachScreenshot(ExtentTest test, string screenshotPath, string stepName)
        {
            if (File.Exists(screenshotPath))
            {
                test.AddScreenCaptureFromPath(screenshotPath, stepName);
            }
        }

        public static void AttachScreenshotInline(ExtentTest test, byte[] screenshotBytes, string stepName)
        {
            var base64 = Convert.ToBase64String(screenshotBytes);
            test.AddScreenCaptureFromBase64String(base64, stepName);
        }
    }
}
