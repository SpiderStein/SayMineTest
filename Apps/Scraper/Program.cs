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
        private IEnumerable<string> _domains;
        private ILogger _logger;
        public Program(
            IDummyBDAL dummyBDAL,
            IScraperFactory scraperFactory,
            IEnumerable<string> domains,
            ILogger logger
        )
        {
            this._dummyBDAL = dummyBDAL;
            this._scraperFactory = scraperFactory;
            this._domains = domains;
            this._logger = logger;
        }

        public async Task Run()
        {
            var execTimer = Stopwatch.StartNew();
            try
            {
                var tasks = new List<Task>();
                foreach (var d in this._domains)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var scraper = this._scraperFactory.Get();
                        var scrapeResult = await scraper.GetPrivacyRelatedEmails(d).ConfigureAwait(false);
                        await this._dummyBDAL.Insert(scrapeResult).ConfigureAwait(false);
                    }));
                }
                await Task.WhenAll(tasks);
            }
            catch (System.Exception ex)
            {
                this._logger.Fatal(ex.ToString());
                throw;
            }
            execTimer.Stop();
            this._logger.Information(execTimer.Elapsed.TotalMilliseconds.ToString());
        }
    }
}