namespace CompatibilityAnalyzer
{
    public struct FrameworkInfo
    {
        public FrameworkInfo(string framework, System.Version version, string folderName)
        {
            Framework = framework;
            Version = version;
            FolderName = folderName;
        }

        public string Framework { get; }

        public System.Version Version { get; }

        public string FolderName { get; }

        public override string ToString() => $"{Framework}, {Version}";
    }
}
