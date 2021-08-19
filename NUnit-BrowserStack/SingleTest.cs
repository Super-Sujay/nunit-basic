using NUnit.Framework;
using OpenQA.Selenium;

namespace BrowserStack
{
    [TestFixture("single", "firefox")]
    public class SingleTest : BrowserStackNUnitTest
    {
        public SingleTest(string profile, string environment) : base(profile, environment) { }

        [Test]
        public void SearchGoogle()
        {
            IWebDriver driver = GetWebDriver();
            driver.Navigate().GoToUrl("https://www.google.com/ncr");
            IWebElement query = driver.FindElement(By.Name("q"));
            query.SendKeys("BrowserStack");
            query.Submit();
            System.Threading.Thread.Sleep(5000);
            Assert.AreEqual("BrowserStack - Google Search", driver.Title);
        }
    }
}
