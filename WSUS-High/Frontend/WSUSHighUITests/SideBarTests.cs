using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace WSUSHighUITest
{
    [TestFixture]
    public class SideBarTests
    {
        private static ChromeDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
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
        public void TestSwitchPage()
        {
            driver.Navigate().GoToUrl("http://localhost:3001");

            // Find and click the Clients button using ExpectedConditions
            var clientsButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[data-testid='clientsBtn']")));
            clientsButton.Click();

            // Wait for the page title to be visible using ExpectedConditions
            var pageTitle = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h3[data-testid='pageTitle']")));

            // Assert that the page title text contains "Clients"
            Assert.That(pageTitle.Text, Does.Contain("Clients"), "Failed to navigate to the Clients page.");
        }
    }
}