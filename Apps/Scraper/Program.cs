using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Scraper
{
    public class Program
    {
        private IDummyBDAL _dummyBDAL;
        private IScraper _scraper;
        private IEnumerable<string> _domains;
        public Program(
            IDummyBDAL dummyBDAL,
            IScraper scraper,
            IEnumerable<string> domains
        )
        {
            this._dummyBDAL = dummyBDAL;
            this._scraper = scraper;
            this._domains = domains;
        }

        public Task Run()
        {
            throw new NotImplementedException();
        }
    }
}