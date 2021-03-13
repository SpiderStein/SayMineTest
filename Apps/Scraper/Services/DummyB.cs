using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper
{
    public class DummyB : IDummyBDAL
    {
        // private static readonly BlockingCollection<ScrapeResult>
        public Task Insert(IEnumerable<ScrapeResult> scrapeResults)
        {
            throw new System.NotImplementedException();
        }
    }
}