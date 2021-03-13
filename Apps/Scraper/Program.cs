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
        private IScraper _scraper;
        private IEnumerable<string> _domains;
        private ILogger _logger;
        public Program(
            IDummyBDAL dummyBDAL,
            IScraper scraper,
            IEnumerable<string> domains,
            ILogger logger
        )
        {
            this._dummyBDAL = dummyBDAL;
            this._scraper = scraper;
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
                    tasks.Add(this._scraper.GetPrivacyRelatedEmails(d).ContinueWith(async (scrapeResult) =>
                    {
                        await this._dummyBDAL.Insert(await scrapeResult);
                    }));
                }
                await Task.WhenAll(tasks);
            }
            catch (System.Exception ex)
            {
                this._logger.Fatal(ex.ToString());
            }
        }
    }
}