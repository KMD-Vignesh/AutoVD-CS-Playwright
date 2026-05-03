using NUnit.Framework;
using AutoVD.Framework.Reports;

namespace AutoVD.Tests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            var _ = ExtentReportManager.Instance;
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            ExtentReportManager.Close();
        }
    }
}
