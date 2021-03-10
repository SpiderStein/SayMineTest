using System;
using System.Collections.Generic;
using System.Linq;

namespace Scraper
{
    public class ScrapeResult
    {
        public Guid ID { get; set; }
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
        /// <summary>
        /// The time that the scraping process took. The value is authentic only if the scraping process
        /// isn't interrupted by, for example, time limit to scrape the domain.
        /// </summary>
        /// <value></value>
        public TimeSpan ElapsedTime { get; set; }




        public override string ToString()
        {
            return string.Join(", ",
                EmailAddresses?.Select(x => $"address: {x.EmailAddress} url: {x.FoundInUrl}") ?? Enumerable.Empty<string>());
        }
    }
}