using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace Assignment
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        private static readonly BlockingCollection<ScrapeResult>
            _scrapeResults = new BlockingCollection<ScrapeResult>();

        private static IEnumerable<string> _domains;
        
        public static void Main(string[] args)
        {
            _domains = LoadDomainsFromFile();
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            ScrapeAllDomains();
            
            stopwatch.Stop();

            PrintScrapeResult(stopwatch);
        }

        private static void ScrapeAllDomains()
        {
            // TODO:
            // Initialize scraper and run on all entries in _domains
            // This method must store the results in _scrapeResults
            
            throw new NotImplementedException();
        }

        private static void PrintScrapeResult(Stopwatch stopwatch)
        {
            Console.WriteLine($"Scraping {_domains.Count()} domains took: {stopwatch.Elapsed}");
            
            // print first results
            foreach (var scrapeResult in _scrapeResults.Take(10))
            {
                Console.WriteLine($"domain: {scrapeResult.Domain} emails: {scrapeResult}");
            }
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