using System.Collections.Generic;

namespace AutoVD.Framework.Models
{
    public class AppSettings
    {
        public string BaseUrl { get; set; } = null!;
        public List<string> BrowserOptions { get; set; } = null!;
        public int DefaultTimeout { get; set; }
        public int SlowMo { get; set; }
        public bool Headless { get; set; }
        public string ScreenshotPath { get; set; } = null!;
        public string ReportPath { get; set; } = null!;
        public int RetryCount { get; set; }
    }
}
