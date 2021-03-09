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

        public Task<ScrapeResult> GetPrivacyRelatedEmails(string domain) // I should save immediately after getting the result, outside of the call.
        {
            throw new NotImplementedException();
            try
            {

            }
            catch (ElementNotInteractableException) { } // Continue if this error is occured.
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