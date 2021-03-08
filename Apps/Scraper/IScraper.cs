using System.Threading.Tasks;

namespace Assignment
{
    public interface IScraper
    {
        /// <summary>
        /// Scrapes the domain for email addresses
        /// </summary>
        /// <param name="domain">The domain to scrape</param>
        Task<ScrapeResult> Scrape(string domain);
    }
}