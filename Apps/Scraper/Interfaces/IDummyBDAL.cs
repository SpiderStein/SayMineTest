using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper
{
    public interface IDummyBDAL
    {
        Task Insert(IEnumerable<ScrapeResult> scrapeResults);
    }
}