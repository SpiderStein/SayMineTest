using OpenQA.Selenium.Chrome;
using Serilog;

namespace Scraper
{
    public class ChromeScraperFactory : IScraperFactory
    {
        private ChromeOptions _chromeOptions;
        private Translator _translator;
        private ILogger _logger;
        private byte _amountOfHopsAllowedFromDomain;
        private string _chromeDriverLocation;

        public ChromeScraperFactory(
            ChromeOptions chromeOptions,
            Translator translator,
            ILogger logger,
            byte amountOfHopsAllowedFromDomain,
            string chromeDriverLocation
        )
        {
            this._chromeOptions = chromeOptions;
            this._translator = translator;
            this._logger = logger;
            this._amountOfHopsAllowedFromDomain = amountOfHopsAllowedFromDomain;
            this._chromeDriverLocation = chromeDriverLocation;
        }
        public IScraper Get()
        {
            var chromeDriver = new ChromeDriver(this._chromeDriverLocation, this._chromeOptions);
            return new ChromeScraper(chromeDriver, this._translator, this._logger, this._amountOfHopsAllowedFromDomain);
        }
    }
}