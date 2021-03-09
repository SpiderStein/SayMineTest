namespace Scraper
{
    public class Translator
    {
        private string _secret;
        private string _endpoint;
        private string _location;

        public Translator(
            string secret,
            string endpoint,
            string location
        )
        {
            this._secret = secret;
            this._endpoint = endpoint;
            this._location = location;
        }
    }
}