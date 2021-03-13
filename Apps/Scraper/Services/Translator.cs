using System.Threading;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Scraper
{
    public class Translator
    {
        private string _secret;
        private string _endpoint;
        private string _region;
        private HttpClient _client;
        private string _translateRoute;
        private string _detectLangRoute;
        private ILogger log;

        public Translator(
            string secret,
            string endpoint,
            string region,
            HttpClient translationServiceClient,
            string translateFuncRoute,
            string detectLangFuncRoute,
            ILogger logger
        )
        {
            this._secret = secret;
            this._endpoint = endpoint;
            this._region = region;
            this._client = translationServiceClient;
            this._translateRoute = translateFuncRoute;
            this._detectLangRoute = detectLangFuncRoute;
            this.log = logger;
        }

        /// <summary>
        /// Translate the input from its language to the desired language.
        /// </summary>
        /// <param name="from">The input's language name.</param>
        /// <param name="to">The name of the language that the input will be translated to.</param>
        /// <exception cref="OperationCanceledException">When the Translator service is unresponsive.</exception>
        /// <exception cref="Exception"></exception>
        public Task<TranslationResult> Translate(string from, string to, string input)
        {
            return Task.Run<TranslationResult>(async () =>
            {
                var reqBody = JsonConvert.SerializeObject(new[] { new { Text = input } });
                using var request = new HttpRequestMessage();
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(this._endpoint + this._translateRoute);
                request.Content = new StringContent(reqBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", this._secret);
                request.Headers.Add("Ocp-Apim-Subscription-Region", this._region);
                using var tokenSrc = new CancellationTokenSource(5000);
                HttpResponseMessage response;
                try
                {
                    response = await this._client.SendAsync(request, tokenSrc.Token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var agEx = ex as AggregateException;
                    if (agEx != null && agEx.InnerExceptions.Count == 1 && agEx.InnerException is OperationCanceledException)
                    {
                        this.log.Fatal("\"Translator\" service is unresponsive");
                        throw agEx;
                    }
                    throw;
                }
                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TranslationResult[]>(result)[0];
            });
        }

        public Task<DetectResult> DetectLang(string input)
        {
            return Task.Run<DetectResult>(async () =>
            {

                var reqBody = JsonConvert.SerializeObject(new[] { new { Text = input } });
                using var request = new HttpRequestMessage();
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(this._endpoint + this._detectLangRoute);
                request.Content = new StringContent(reqBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", this._secret);
                using var tokenSrc = new CancellationTokenSource(5000);
                HttpResponseMessage response;
                try
                {
                    response = await this._client.SendAsync(request, tokenSrc.Token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var agEx = ex as AggregateException;
                    if (agEx != null && agEx.InnerExceptions.Count == 1 && agEx.InnerException is OperationCanceledException)
                    {
                        this.log.Fatal("\"Translator\" service is unresponsive");
                        throw agEx;
                    }
                    throw;
                }
                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DetectResult[]>(result)[0];
            });
        }
    }
}