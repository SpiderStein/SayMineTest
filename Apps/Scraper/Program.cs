using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Scraper
{
    public class Program
    {
        private IDummyBDAL _dummyBDAL;
        private IScraperFactory _scraperFactory;
        private string[] _domains;
        private ILogger _logger;
        private Byte _amountOfCores;
        public Program(
            IDummyBDAL dummyBDAL,
            IScraperFactory scraperFactory,
            string[] domains,
            ILogger logger,
            byte amountOfCores
        )
        {
            this._dummyBDAL = dummyBDAL;
            this._scraperFactory = scraperFactory;
            this._domains = domains;
            this._logger = logger;
            this._amountOfCores = amountOfCores;
        }

        public async Task Run()
        {
            var execTimer = Stopwatch.StartNew();
            try
            {
                var tasks = new List<Task>();
                for (int index = 0; index < this._domains.Length; index += _amountOfCores)
                {
                    if (index + this._amountOfCores >= this._domains.Length)
                    {
                        this._domains[index..].AsParallel().ForAll((d) =>
                        {
                            this.addScrapeTaskAndRun(tasks, d);
                        });
                        await Task.WhenAll(tasks);
                    }
                    else
                    {
                        this._domains[index..(index + this._amountOfCores)].AsParallel().ForAll((d) =>
                        {
                            this.addScrapeTaskAndRun(tasks, d);
                        });
                        await Task.WhenAll(tasks);
                        tasks.Clear();
                    }
                }
            }
            catch (System.Exception ex)
            {
                this._logger.Fatal(ex.ToString());
                throw;
            }
            execTimer.Stop();
            this._logger.Information($"The time it took to scrape all the domains is the following: { execTimer.Elapsed.TotalMilliseconds.ToString()}");
        }

        private void addScrapeTaskAndRun(List<Task> tasks, string domainToScrape)
        {
            tasks.Add(
                Task.Run(async () =>
                {
                    var scraper = this._scraperFactory.Get();
                    this._logger.Information($"Starting to scrape: \"{domainToScrape}\"");
                    var scrapeResult = await scraper.GetPrivacyRelatedEmails(domainToScrape).ConfigureAwait(false);
                    this._logger.Information($"\"{domainToScrape}\" is scraped{Environment.NewLine}The scraping product is the following:{Environment.NewLine}{scrapeResult.ToString()}");
                    await this._dummyBDAL.Insert(scrapeResult).ConfigureAwait(false);
                    this._logger.Information($"\"{domainToScrape}\" is saved in DummyB");
                }));
        }
    }
}