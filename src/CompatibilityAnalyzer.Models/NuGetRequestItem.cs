namespace CompatibilityAnalyzer.Models
{
    public class NuGetRequestItem : IRequestItem
    {
        public string Feed { get; set; }

        public string Id { get; set; }

        public string Version { get; set; }
    }
}
