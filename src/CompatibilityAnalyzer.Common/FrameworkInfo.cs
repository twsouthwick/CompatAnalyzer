namespace CompatibilityAnalyzer
{
    public struct FrameworkInfo
    {
        public FrameworkInfo(string framework, System.Version version)
        {
            Framework = framework;
            Version = version;
        }

        public string Framework { get; }

        public System.Version Version { get; }

        public override string ToString() => $"{Framework}, {Version}";
    }
}