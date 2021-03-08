using System;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;

namespace ScraperTests
{
    public class SeleniumExample
    {
        [Test]
        public void RunSeleniumExample()
        {
            // Note: you must have Chrome 84 installed on your machine in order to run this

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--incognito");
            chromeOptions.AddArgument("--disable-plugins-discovery");
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--window-size=1920,1200");
            chromeOptions.AddArgument("--user-agent=\"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36\"");
            
            var webDriver = new ChromeDriver(Directory.GetCurrentDirectory(), chromeOptions);
            
            webDriver.Navigate().GoToUrl("https://saymine.com");

            var title = webDriver.Title;
            Console.WriteLine($"Got :{title}");
            
            Assert.True(title.Contains("Mine"));
        }
    }
}