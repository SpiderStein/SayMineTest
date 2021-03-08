using System.Collections.Generic;
using System.Linq;

namespace Assignment
{
    public class ScrapeResult
    {
        public string Domain { get; set; }
        
        public ICollection<ScrapedEmailAddress> EmailAddresses { get; set; }

        public override string ToString()
        {
            return string.Join(", ", 
                EmailAddresses?.Select(x => $"address: {x.EmailAddress} url: {x.FoundInUrl}") ?? Enumerable.Empty<string>());
        }
    }
}