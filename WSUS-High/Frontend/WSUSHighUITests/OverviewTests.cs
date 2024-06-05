using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;


namespace WSUSHighUITests
{
    [TestFixture]
    public class OverviewTests
    {
        private static ChromeDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
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

            // Find the refresh button
            var refreshButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[data-testid='refreshBtn']")));
            refreshButton.Click();

            // Wait for either the refresh icon to disappear or a specific element to appear 
            wait.Until(d =>
            {
                try
                {
                    return !d.FindElement(By.CssSelector("svg[data-testid='refreshIcon']")).Displayed;
                }
                catch (NoSuchElementException)
                {
                    return true; // The icon is no longer displayed
                }
            });

            // Re-locate the API status element (it might have been replaced during the refresh)
            var apiStatus = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("span[data-testid='apiStatusResult']")));

            // Assert the updated API status
            Assert.That(apiStatus.Text, Does.Contain("Online"), "WSUS API is not online");
        }
    }
}
