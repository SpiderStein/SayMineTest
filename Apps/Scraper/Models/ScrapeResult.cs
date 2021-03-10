using System.Collections.Generic;
using System.Linq;

namespace Scraper
{
    public class ScrapeResult
    {
        /// <summary>
        /// The url where the scraping process takes place.
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// Scraped email addresses.
        /// </summary>
        public ICollection<ScrapedEmailAddress> EmailAddresses { get; set; }
        /// <summary>
        /// The detected languages from 1 "a" htmlElement textContent (method result) that's sampled. 
        /// </summary>
        public DetectResult LangDetectResult { get; set; }




        public override string ToString()
        {
            return string.Join(", ",
                EmailAddresses?.Select(x => $"address: {x.EmailAddress} url: {x.FoundInUrl}") ?? Enumerable.Empty<string>());
        }
    }
}