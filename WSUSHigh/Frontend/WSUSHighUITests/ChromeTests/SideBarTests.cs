using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace WSUSHighUITests.ChromeTests
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

            var clientsButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[data-testid='clientsBtn']")));
            clientsButton.Click();

            var pageTitle = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h3[data-testid='pageTitle']")));

            Assert.That(pageTitle.Text, Does.Contain("Clients"), "Failed to navigate to the Clients page.");
        }
    }
}