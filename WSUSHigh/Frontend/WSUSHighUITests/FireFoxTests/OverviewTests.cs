using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Firefox;

namespace WSUSHighUITests.FireFoxTests
{
    [TestFixture]
    public class OverviewTests
    {
        private static FirefoxDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new FirefoxDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }

        [Test]
        public void TestRefreshButton()
        {
            driver.Navigate().GoToUrl("http://localhost:3001");
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[data-testid='overviewBtn']"))).Click();

            var refreshButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[data-testid='refreshBtn']")));
            refreshButton.Click();

            wait.Until(d =>
            {
                try
                {
                    return !d.FindElement(By.CssSelector("svg[data-testid='refreshIcon']")).Displayed;
                }
                catch (NoSuchElementException)
                {
                    return true;
                }
            });

            var apiStatus = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[data-testid='apiStatusResult']")));

            Assert.That(apiStatus.Text, Does.Contain("Online"), "WSUS API is not online");
        }
    }
}
