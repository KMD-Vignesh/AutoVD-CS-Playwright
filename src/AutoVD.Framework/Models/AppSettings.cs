using System.Collections.Generic;

namespace AutoVD.Framework.Models
{
    public class AppSettings
    {
        public string BaseUrl { get; set; }
        public List<string> BrowserOptions { get; set; }
        public int DefaultTimeout { get; set; }
        public int SlowMo { get; set; }
        public bool Headless { get; set; }
        public string ScreenshotPath { get; set; }
        public string ReportPath { get; set; }
        public int RetryCount { get; set; }
    }
}
