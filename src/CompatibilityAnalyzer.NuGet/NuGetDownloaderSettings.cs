namespace CompatibilityAnalyzer
{
    public class NuGetDownloaderSettings
    {
        private readonly IAnalysisOptions _options;

        public NuGetDownloaderSettings(IAnalysisOptions options)
        {
            _options = options;
        }

        public string Feed => _options.Feed;
    }
}
