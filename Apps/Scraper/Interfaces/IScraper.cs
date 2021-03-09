using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper
{
    public interface IScraper
    {
        Task<ScrapeResult> GetPrivacyRelatedEmails(string domain);
    }
}