using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System;
using Serilog;
using OpenQA.Selenium;

namespace Scraper
{
    public class ChromeScraper : IScraper
    {
        private Stopwatch _stopwatch;
        private ChromeDriver _driver;
        private ILogger _log;
        private Translator _translator;

        public ChromeScraper(
            Stopwatch stopwatch,
            ChromeDriver chromeDriver,
            Translator translator,
            ILogger logger
        )
        {
            this._stopwatch = stopwatch;
            this._driver = chromeDriver;
            this._translator = translator;
            this._log = logger;
        }







        // I should save immediately after getting the result, outside of the call.
        public Task<ScrapeResult> GetPrivacyRelatedEmails(string domain, byte amountOfHopsAllowedFromDomain)
        {
            return Task.Run<ScrapeResult>(() =>
            {
                try
                {
                    // Urls sorted by the hop that they are discovered at. The array indexes are the hops.
                    var urlsToScrape = new List<string>[amountOfHopsAllowedFromDomain];
                    for (int index = 0; index < urlsToScrape.Length; index++)
                    {
                        urlsToScrape[index] = new List<string>();
                    }

                    for (int hops = 0; hops < amountOfHopsAllowedFromDomain; hops++)
                    {
                        this._driver.Navigate().GoToUrl(domain);
                        // TODO: Continue from here
                        if (hops + 1 == amountOfHopsAllowedFromDomain) // If it's the highest allowed hop.
                        {

                        }
                    }
                }
                catch (System.Exception ex)
                {
                    this._log.Error(ex.ToString());
                    throw;
                }
            })
        }



        private void LogScrapeResults(IEnumerable<string> domains, IEnumerable<ScrapeResult> results)
        {
            this._log.Information($"Scraping {domains.Count()} domains took: {this._stopwatch.Elapsed}");

            // print first results
            foreach (var res in results.Take(10))
            {
                this._log.Information($"domain: {res.Domain} emails: {res}");
            }
        }
    }
}