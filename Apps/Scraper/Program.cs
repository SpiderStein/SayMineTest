using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using OpenQA.Selenium.Chrome;

namespace Scraper
{
    public class Program
    {
        private static readonly BlockingCollection<ScrapeResult>
            _scrapeResults = new BlockingCollection<ScrapeResult>();

        private static IEnumerable<string> _domains;

        public static void Main(string[] args)
        {
            // _domains = LoadDomainsFromFile();

            // var stopwatch = new Stopwatch();
            // stopwatch.Start();

            // ScrapeAllDomains();

            // stopwatch.Stop();

            // PrintScrapeResult(stopwatch);

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--incognito");
            chromeOptions.AddArgument("--disable-plugins-discovery");
            // chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--window-size=1920,1200");
            chromeOptions.AddArgument("--user-agent=\"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36\"");

            var webDriver = new ChromeDriver(Environment.GetEnvironmentVariable("ChromeDriverLocation"), chromeOptions);

            // webDriver.Navigate().GoToUrl("https://saymine.com");

            // var title = webDriver.Title;
            // Console.WriteLine($"Got :{title}");

            webDriver.Navigate().GoToUrl("https://www.espncricinfo.com/");
            Console.WriteLine(webDriver.Url);
            webDriver.FindElementByTagName("a").Click();
            Console.WriteLine(webDriver.Url);


        }

        private static void ScrapeAllDomains()
        {
            // TODO:
            // Initialize scraper and run on all entries in _domains
            // This method must store the results in _scrapeResults

            throw new NotImplementedException();
        }

        private static void PrintScrapeResult(Stopwatch stopwatch)
        {
            Console.WriteLine($"Scraping {_domains.Count()} domains took: {stopwatch.Elapsed}");

            // print first results
            foreach (var scrapeResult in _scrapeResults.Take(10))
            {
                Console.WriteLine($"domain: {scrapeResult.Domain} emails: {scrapeResult}");
            }
        }

        private static IEnumerable<string> LoadDomainsFromFile()
        {
            var listAddress = new List<string>();
            var filePath = Path.Combine(Environment.CurrentDirectory, "domains.csv");

            using var reader = File.OpenText(filePath);
            using var csv = new CsvReader(reader);

            csv.Configuration.HasHeaderRecord = false;

            while (csv.Read())
            {
                listAddress.Add(csv.GetField(0));
            }

            return listAddress;
        }
    }
}