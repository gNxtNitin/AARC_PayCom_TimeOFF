using AngleSharp.Css;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Diagnostics;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace PaycomTimeOffData.BusinessLogic
{
    public enum Browsers
    {
        ChromeBrowser,
        EdgeBrowser
    }

    public static class WebDriverHandler
    {
        private static IWebDriver webdriver;
        private static IConfiguration config;
        public static void LaunchBrowser(Browsers browser, string url, string userDataDir ="", string profile="")
        {
            
            WebDriverInit(browser, userDataDir, profile);
            try
            {
                webdriver.Navigate().GoToUrl(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        private static void WebDriverInit(Browsers browser, string userDataDir, string profile)
        {
            
            switch (browser)
            {
                case Browsers.ChromeBrowser:
                    new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
                    webdriver = GetChromeInstance(userDataDir, profile);

                    break;
                case Browsers.EdgeBrowser:
                    new DriverManager().SetUpDriver(new EdgeConfig(), VersionResolveStrategy.MatchingBrowser);
                    EdgeOptions edgeOptions = new EdgeOptions();
                    webdriver = GetEdgeInstance(userDataDir, profile);
                    break;
            }
            
        }

        private static IWebDriver GetChromeInstance(string userDataDir, string profile)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOptions.AddArgument("--disable-infobars");          
            chromeOptions.AddArgument("--disable-popup-blocking");      
            chromeOptions.AddArgument("--no-sandbox");         
            chromeOptions.AddArgument("--disable-dev-shm-usage");     
            chromeOptions.AddArgument("--start-maximized");           
            chromeOptions.AddArgument("--lang=en-US");          
            //chromeOptions.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
            chromeOptions.AddAdditionalOption("useAutomationExtension", false);
            chromeOptions.AddArgument("--log-level=3");



            if (!string.IsNullOrEmpty(userDataDir))
            {
                chromeOptions.AddArgument($"--user-data-dir={userDataDir}");
                if (!string.IsNullOrEmpty(profile))
                {
                    chromeOptions.AddArgument($"profile-directory={profile}");
                }
            }

            IWebDriver driver = new ChromeDriver(chromeOptions);

            
            //((IJavaScriptExecutor)driver).ExecuteScript(@"
            //    Object.defineProperty(navigator, 'webdriver', {get: () => undefined});
            //    Object.defineProperty(navigator, 'languages', {get: () => ['en-US', 'en']});
            //    Object.defineProperty(navigator, 'plugins', {get: () => [1, 2, 3]});
            //    Object.defineProperty(navigator, 'platform', {get: () => 'Win32'});"
            //);

            return driver;
        }


        private static IWebDriver GetEdgeInstance(string userDataDir, string profile)
        {
            EdgeOptions edgeOptions = new EdgeOptions();

            edgeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            edgeOptions.AddArgument("--window-size=1920,1080");
            edgeOptions.AddArgument("--ignore-certificate-errors");
            edgeOptions.AddArguments("--enable-features=NetworkService,NetworkServiceInProcess");
            edgeOptions.AddArguments("--disable-renderer-backgrounding");

            if (!string.IsNullOrEmpty(userDataDir))
            {
                edgeOptions.AddArgument($"--user-data-dir={userDataDir}");
                if (!string.IsNullOrEmpty(profile))
                {
                    edgeOptions.AddArgument($"profile-directory={profile}");
                }
            }


            IWebDriver driver = new EdgeDriver(edgeOptions);

            return driver;
        }

        public static bool CheckForElement(string selector, int waitTimeout, out IWebElement element, bool checkVisible = true, bool checkExists = true)
        {

            bool isExists = false;
            element = null;
            try
            {
                WebDriverWait waitForElement = new WebDriverWait(webdriver, TimeSpan.FromSeconds(waitTimeout));
                if (checkVisible == true)
                {
                    element = waitForElement.Until(ExpectedConditions.ElementIsVisible(By.XPath(selector)));
                    return true;
                }

                if (checkExists == true)
                {
                    element = waitForElement.Until(ExpectedConditions.ElementExists(By.XPath(selector)));
                    return true;
                }
            }
            catch(Exception ex)
            { 
                element = null;
            }

            return isExists;
            
        }

        public static void PopulateTextInput(string inputSelector, string inputText)
        {
            IWebElement element = null;
            bool isExists = CheckForElement(inputSelector, 10, out element);
            if (isExists == true && element != null)
            {
                element.SendKeys(inputText);
            }
        }

        public static void ClickOnElement(string selector, bool checkVisible = true, bool checkExists = true)
        {
            IWebElement element = null;
            bool isExists = CheckForElement(selector, 10, out element, checkVisible, checkExists);
            if (isExists == true && element != null)
            {
                element.Click();
            }
        }

        public static string GetPageSource()
        {
            return webdriver.PageSource;
        }

        public static string GetInnerText(string selector)
        {
            IWebElement element = null;
            string innerText = string.Empty;

            bool isExists = CheckForElement(selector, 10, out element);

            if(isExists == true && element != null)
            {
                innerText = System.Net.WebUtility.HtmlDecode(element.Text.Trim());
            }

            return innerText;
        }

        public static void NavigateToUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                webdriver.Navigate().GoToUrl(url);
            }
           
        }

        public static void CloseBrowser()
        {
            if(webdriver != null)
            {
                webdriver.Quit();
            }
        }

        public static void CreateLog(string Path,string message)
        {
            try
            {
                string filePath = Path;

                using (StreamWriter writer = new StreamWriter(filePath, append: true))
                {
                    writer.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
