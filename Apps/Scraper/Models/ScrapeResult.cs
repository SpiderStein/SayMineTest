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

        public ICollection<ScrapedEmailAddress> EmailAddresses { get; set; }

        public override string ToString()
        {
            return string.Join(", ",
                EmailAddresses?.Select(x => $"address: {x.EmailAddress} url: {x.FoundInUrl}") ?? Enumerable.Empty<string>());
        }
    }
}