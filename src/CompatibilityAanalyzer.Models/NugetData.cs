namespace CompatibilityAnalyzer.Models
{
    public class NugetData : IRequestItem
    {
        public string Feed { get; set; }

        public string Id { get; set; }

        public string Version { get; set; }
    }
}
