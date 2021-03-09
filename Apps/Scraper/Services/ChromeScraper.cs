using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System;
using Serilog;

namespace Scraper
{
    public class ChromeScraper : IScraper
    {
        private Stopwatch _stopwatch;
        private ChromeDriver _driver;
        private TranslatorDeps _transDeps;
        private ILogger _log;

        public Task<IEnumerable<ScrapeResult>> GetPrivacyRelatedEmails(IEnumerable<string> domains)
        {
            throw new System.NotImplementedException();
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