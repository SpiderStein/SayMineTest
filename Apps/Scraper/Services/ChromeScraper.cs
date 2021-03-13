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

namespace Scraper
{
    public class ChromeScraper : IScraper
    {
        private ChromeDriver _driver;
        private ILogger _log;
        private Translator _translator;
        byte _amountOfHopsAllowedFromDomain;

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







        // I should save immediately after getting the result, outside of the call.
        /// <returns>The value can be null</returns>
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




                    var detectRes = await _translator.DetectLang(elems[0].Text).ConfigureAwait(false);
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
                    if (this._amountOfHopsAllowedFromDomain == 0)
                    {
                        foreach (var elem in scrapedElems)
                        {
                            var textWords = elem.Text.Split();
                            foreach (var w in textWords)
                            {
                                if (w.Contains(@"@"))
                                {
                                    if (!scrapedEmails.ContainsKey(w))
                                    {
                                        // I take the assumption that it's well tested, and thus won't hurt the reliability of the unit testing.
                                        scrapedEmails.Add(w, new ScrapedEmailAddress { EmailAddress = w, FoundInUrl = domain, IsValid = EmailValidator.Validate(w, true, true) });
                                    }
                                }
                            }
                        }
                        execTimer.Stop();
                        return new ScrapeResult { Domain = domain, ElapsedTime = execTimer.Elapsed, EmailAddresses = scrapedEmails.Values, ID = Guid.NewGuid(), LangDetectResult = detectRes };
                    }





                    for (int hops = 0; hops < this._amountOfHopsAllowedFromDomain; hops++)
                    {
                        if (hops + 1 == this._amountOfHopsAllowedFromDomain) // If it's the highest allowed hop.
                        {

                        }
                    }
                }
                catch (System.Exception ex)
                {
                    this._log.Error(ex.ToString());
                    throw;
                }
            })
        }



        private void LogScrapeResults(IEnumerable<string> domains, IEnumerable<ScrapeResult> results)
        {
            this._log.Information($"Scraping {domains.Count()} domains took: {this._stopwatch.Elapsed}");

            // print first results
            foreach (var res in results.Take(10))
            {
                this._log.Information($"domain: {res.Domain} emails: {res}");
            }
        }
    }
}