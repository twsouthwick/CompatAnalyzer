namespace CompatibilityAnalyzer
{
    public struct FrameworkInfo
    {
        public FrameworkInfo(string framework)
        {
            Framework = framework;
        }

        public string Framework { get; }
    }
}