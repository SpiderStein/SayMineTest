using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace Scraper
{
    public class DummyB : IDummyBDAL, IDisposable
    {
        private BlockingCollection<ScrapeResult> _scrapeResults;
        private ILogger _logger;
        private bool _disposed = false;

        public DummyB(
            BlockingCollection<ScrapeResult> scrapeResults,
            ILogger logger
        )
        {
            this._scrapeResults = scrapeResults;
            this._logger = logger;
        }

        public Task Insert(ScrapeResult scrapeResult)
        {
            return Task.Run(() =>
            {
                _logger.Debug($"Inserting to DummyB, \"{scrapeResult.Domain}\" scraping product");
                this._scrapeResults.Add(scrapeResult);
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            this._scrapeResults.Dispose();
        }

        ~DummyB() => this.Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}