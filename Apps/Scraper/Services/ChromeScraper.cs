using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System;
using Serilog;
using OpenQA.Selenium;
using EmailValidation;
using System.Collections.ObjectModel;

namespace Scraper
{
    public class ChromeScraper : IScraper, IDisposable
    {
        private ChromeDriver _driver;
        private ILogger _log;
        private Translator _translator;
        private byte _amountOfHopsAllowedFromDomain;
        private bool _disposed = false;

        public ChromeScraper(
            ChromeDriver chromeDriver,
            Translator translator,
            ILogger logger,
            byte amountOfHopsAllowedFromDomain
        )
        {
            this._driver = chromeDriver;
            this._translator = translator;
            this._log = logger;
            this._amountOfHopsAllowedFromDomain = amountOfHopsAllowedFromDomain;
        }








        public Task<ScrapeResult> GetPrivacyRelatedEmails(string domain)
        {
            return Task.Run<ScrapeResult>(async () =>
            {
                try
                {
                    var execTimer = Stopwatch.StartNew();

                    this._driver.Navigate().GoToUrl(domain);
                    var elems = this._driver.FindElementsByXPath("//body//*[normalize-space(text()) and not(self::noscript) and not(self::style) and not(self::script)]");
                    if (elems.Count == 0)
                    {
                        this._log.Error($"\"{domain}\" doesn't have elems that contain text. Thus there's nothing to scrape.");
                        execTimer.Stop();
                        return new ScrapeResult { Domain = domain, ID = Guid.NewGuid(), ElapsedTime = execTimer.Elapsed };
                    }




                    var detectRes = await _translator.DetectLang(elems[0].GetAttribute("textContent")).ConfigureAwait(false);
                    var docLang = detectRes.Language;
                    StringBuilder privacyInDocLang;
                    if (docLang != "en")
                    {
                        privacyInDocLang = new StringBuilder((await this._translator.Translate("en", docLang, "privacy").ConfigureAwait(false)).Translations[0].To);
                    }
                    else
                    {
                        privacyInDocLang = new StringBuilder("privacy");
                    }
                    (string lower, string upper, string pascal) casingVariants;
                    casingVariants.lower = privacyInDocLang.ToString().ToLower(); // It's not documented in azure translator service, if lower cased output is returned for lower cased input.
                    casingVariants.upper = privacyInDocLang.ToString().ToUpper();
                    privacyInDocLang[0] = char.ToUpper(privacyInDocLang[0]);
                    casingVariants.pascal = privacyInDocLang.ToString();
                    var xpath = $"//body//a[contains(text(),'{casingVariants.lower}')] | //body//a[contains(text(),'{casingVariants.pascal}')] | //body//a[contains(text(),'{casingVariants.upper}')]";



                    var scrapedElems = this._driver.FindElementsByXPath(xpath);
                    var scrapedEmails = new Dictionary<string, ScrapedEmailAddress>(); // Dictionary instead of hashset, so extending "EqualityComparer<T>" isn't needed.


                    if (_amountOfHopsAllowedFromDomain == 0)
                    {
                        foreach (var elem in scrapedElems)
                        {
                            this.collectEmailsFromText(scrapedEmails, elem.GetAttribute("textContent").Split(), domain);
                        }
                        execTimer.Stop();
                        return new ScrapeResult { Domain = domain, ElapsedTime = execTimer.Elapsed, EmailAddresses = scrapedEmails.Values, ID = Guid.NewGuid(), LangDetectResult = detectRes };
                    }


                    var scrapedUrls = new HashSet<string>[this._amountOfHopsAllowedFromDomain];
                    for (int index = 0; index < scrapedUrls.Length; index++)
                    {
                        scrapedUrls[index] = new HashSet<string>();
                    }
                    this.collectEmailsAndUrlsFromAElems(scrapedEmails, scrapedUrls[0], scrapedElems, domain);

                    for (int hop = 1; hop < this._amountOfHopsAllowedFromDomain - 1; hop++)
                    {
                        foreach (var url in scrapedUrls[hop - 1])
                        {
                            this._driver.Navigate().GoToUrl(url);
                            scrapedElems = this._driver.FindElementsByXPath(xpath);
                            this.collectEmailsAndUrlsFromAElems(scrapedEmails, scrapedUrls[hop], scrapedElems, url);
                        }
                    }
                    collectLastHopEmails(xpath, scrapedElems, scrapedEmails, scrapedUrls);

                    execTimer.Stop();
                    return new ScrapeResult { Domain = domain, ElapsedTime = execTimer.Elapsed, EmailAddresses = scrapedEmails.Values, ID = Guid.NewGuid(), LangDetectResult = detectRes };
                }
                catch (System.Exception ex)
                {
                    this._log.Error(ex.ToString());
                    throw;
                }
            });
        }

        private void collectLastHopEmails(string xpath, ReadOnlyCollection<IWebElement> scrapedElems, Dictionary<string, ScrapedEmailAddress> scrapedEmails, HashSet<string>[] scrapedUrls)
        {
            foreach (var url in scrapedUrls[this._amountOfHopsAllowedFromDomain - 2])
            {
                this._driver.Navigate().GoToUrl(url);
                scrapedElems = this._driver.FindElementsByXPath(xpath);
                foreach (var elem in scrapedElems)
                {
                    this.collectEmailsFromText(scrapedEmails, elem.GetAttribute("textContent").Split(), url);
                }
            }
        }

        //  I chose to use Dictionary instead of hashset, so extending "EqualityComparer<T>" isn't needed.
        /// <summary>
        /// Fills the provided collection with emails that are found in the given text.
        /// If email duplicate is found, no action is done on the collection.
        /// </summary>
        /// <returns>"True" if emails are found.</returns>
        private bool collectEmailsFromText(Dictionary<string, ScrapedEmailAddress> collectionToBeFilled, string[] text, string urlOfTextWebPage)
        {
            var isEmailFound = false;
            foreach (var word in text)
            {
                if (word.Contains(@"@"))
                {
                    isEmailFound = true;
                    if (!collectionToBeFilled.ContainsKey(word))
                    {
                        // I take the assumption that it's well tested, and thus won't hurt the reliability of the unit testing.
                        collectionToBeFilled.Add(word, new ScrapedEmailAddress { EmailAddress = word, FoundInUrl = urlOfTextWebPage, IsValid = EmailValidator.Validate(word, true, true) });
                    }
                }
            }
            return isEmailFound;
        }

        private void collectEmailsAndUrlsFromAElems(
            Dictionary<string, ScrapedEmailAddress> emailsCollToBeFilled, HashSet<string> urlsCollToBeFilled, ReadOnlyCollection<IWebElement> aElems, string elemPageUrl)
        {
            foreach (var elem in aElems)
            {
                var isHrefEmail = false;
                isHrefEmail = this.collectEmailsFromText(emailsCollToBeFilled, elem.GetAttribute("textContent").Split(), elemPageUrl);
                if (!isHrefEmail)
                {
                    urlsCollToBeFilled.Add(elem.GetAttribute("href"));
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            this._driver.Quit();
        }
        ~ChromeScraper() => this.Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}