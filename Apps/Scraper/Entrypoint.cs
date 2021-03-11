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
        // TODO: Validate the configuration if there's a spare time.
        public async static Task Main()
        {
            // _domains = LoadDomainsFromFile();

            // var stopwatch = new Stopwatch();
            // stopwatch.Start();

            // ScrapeAllDomains();

            // stopwatch.Stop();

            // PrintScrapeResult(stopwatch);

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--incognito");
            chromeOptions.AddArgument("--disable-plugins-discovery");
            // chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--window-size=1920,1200");
            chromeOptions.AddArgument("--user-agent=\"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36\"");

            var webDriver = new ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DRIVER_LOCATION"), chromeOptions);
            webDriver.Navigate().GoToUrl("https://www.sports-reference.com/privacy.html");
            System.Console.WriteLine(webDriver.Title);

            var elems = webDriver.FindElements(By.XPath("//*[contains(text(),'privacy')] | //*[contains(text(),'Privacy')] | //*[contains(text(),'PRIVACY')]"));
            System.Console.WriteLine(webDriver.WindowHandles.Count());
            elems[0].Click();
            System.Console.WriteLine(webDriver.WindowHandles.Count());

            // Looks like there's no matches func supported

            // var subKey = "d0c6f0d8b1fb49899eb2af3b21f90ae3";
            // var endpoint = "https://api.cognitive.microsofttranslator.com/";
            // var location = "global";
            // // Output languages are defined as parameters, input language detected.
            // string route = "/translate?api-version=3.0&to=en";
            // string textToTranslate = "פרטיות";
            // object[] body = new object[] { new { Text = textToTranslate } };
            // var requestBody = JsonConvert.SerializeObject(body);

            // using (var client = new HttpClient())
            // using (var request = new HttpRequestMessage())
            // {
            //     // Build the request.
            //     request.Method = HttpMethod.Post;
            //     request.RequestUri = new Uri(endpoint + route);
            //     request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            //     request.Headers.Add("Ocp-Apim-Subscription-Key", subKey);
            //     request.Headers.Add("Ocp-Apim-Subscription-Region", location);

            //     // Send the request and get response.
            //     HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
            //     // Read response as a string.
            //     string result = await response.Content.ReadAsStringAsync();
            //     Console.WriteLine(result);
            // }
        }

        // public static async Task Main()
        // {
        //     using var logger = new LoggerConfiguration()
        //         .WriteTo.Console()
        //         .CreateLogger();

        //     logger.Fatal("TJ and BEAST the cats started to run after the mouse clicks in the web.. hopefully they'll hunt privacy related emails..");
        //     logger.Fatal(Environment.NewLine + File.ReadAllText(Environment.GetEnvironmentVariable("TJ")));
        //     logger.Fatal(Environment.NewLine + File.ReadAllText(Environment.GetEnvironmentVariable("BEAST")));

        //     EmailValidation.EmailValidator.Validate()
        // }



        private static void ScrapeAllDomains()
        {
            // TODO:
            // Initialize scraper and run on all entries in _domains
            // This method must store the results in _scrapeResults

            throw new NotImplementedException();
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