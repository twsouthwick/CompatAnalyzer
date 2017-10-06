namespace CompatibilityAnalyzer
{
    public class PackageIdentity
    {
        public PackageIdentity(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; }

        public string Version { get; }
    }
}
