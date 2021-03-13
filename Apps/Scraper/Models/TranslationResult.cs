namespace Scraper
{
    public class TranslationResult
    {
        public DetectedLang DetectedLanguage { get; set; }
        public TextResult SourceText { get; set; }
        public Translation[] Translations { get; set; }
    }
}