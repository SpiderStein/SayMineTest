namespace Scraper
{
    // I've copied it from an example. Why don't Microsoft guys provide the models in a nuget, for the detection service ?
    public class DetectResult
    {
        public string Language { get; set; }
        public float Score { get; set; }
        public bool IsTranslationSupported { get; set; }
        public bool IsTransliterationSupported { get; set; }
        public AltTranslations[] Alternatives { get; set; }
    }
}