using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper
{
    public interface IScraper
    {
        Task<IEnumerable<ScrapeResult>> GetPrivacyRelatedEmails(IEnumerable<string> domains);
    }
}