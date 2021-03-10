namespace Scraper
{
    public class ScrapedEmailAddress
    {
        /// <summary>
        /// The email address, eg: mailbox@domain.com
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// The full URL in which the email address was found, eg: https://domain.com/contact-us
        /// </summary>
        public string FoundInUrl { get; set; }
        /// <summary>
        /// Did it pass validation.
        /// </summary>
        public bool IsValid { get; set; }
    }
}