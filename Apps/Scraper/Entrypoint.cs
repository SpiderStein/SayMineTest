using System.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Serilog;

namespace Scraper
{
    public class Entrypoint
    {
        public static async Task Main()
        {
            using var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            logger.Fatal("TJ and BEAST the cats started to run after the mouse clicks in the web.. hopefully they'll hunt privacy related emails..");
            logger.Fatal(Environment.NewLine + File.ReadAllText(Environment.GetEnvironmentVariable("TJ")));
            logger.Fatal(Environment.NewLine + File.ReadAllText(Environment.GetEnvironmentVariable("BEAST")));
            await Task.Delay(5000);

            var domains = LoadDomainsFromFile();

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--incognito");
            chromeOptions.AddArgument("--disable-plugins-discovery");
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--window-size=1920,1200");
            chromeOptions.AddArgument("--user-agent=\"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36\"");
            var chromeDriverLocation = Environment.GetEnvironmentVariable("CHROME_DRIVER_LOCATION");

            var translatorSecret = Environment.GetEnvironmentVariable("TRANSLATOR_SECRET");
            var translatorEndpoint = Environment.GetEnvironmentVariable("TRANSLATOR_ENDPOINT");
            var translatorRegion = Environment.GetEnvironmentVariable("TRANSLATOR_REGION");
            var translatorTranslateFuncRoute = Environment.GetEnvironmentVariable("TRANSLATOR_TRANSLATE_FUNC_ROUTE");
            var translatorDetectLangFuncRoute = Environment.GetEnvironmentVariable("TRANSLATOR_DETECT_LANG_FUNC_ROUTE");
            using var translationServiceClient = new HttpClient();
            var translator = new Translator(translatorSecret, translatorEndpoint, translatorRegion,
                translationServiceClient, translatorTranslateFuncRoute, translatorDetectLangFuncRoute, logger);

            var amountOfHopsAllowedFromDomain = Byte.Parse(Environment.GetEnvironmentVariable("AMOUNT_OF_HOPS_ALLOWED_FROM_GIVEN_DOMAIN"));

            var chromeScraperFactory = new ChromeScraperFactory(chromeOptions, translator, logger, amountOfHopsAllowedFromDomain, chromeDriverLocation);

            using var db = new DummyB(new BlockingCollection<ScrapeResult>(), logger);
            var program = new Program(db, chromeScraperFactory, domains, logger);
            await program.Run().ConfigureAwait(false);
        }

        private static IEnumerable<string> LoadDomainsFromFile()
        {
            var listAddress = new List<string>();
            var filePath = Path.Combine(Environment.CurrentDirectory, "domains.csv");

            using var reader = File.OpenText(filePath);
            using var csv = new CsvReader(reader);

            csv.Configuration.HasHeaderRecord = false;

            while (csv.Read())
            {
                listAddress.Add(csv.GetField(0));
            }

            return listAddress;
        }
    }
}