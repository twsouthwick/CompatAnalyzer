using NuGet.Frameworks;

namespace CompatibilityAnalyzer
{
    internal class WrappedNuGetFramework : INuGetFramework
    {
        public WrappedNuGetFramework(NuGetFramework framework)
        {
            Framework = framework;
        }

        public NuGetFramework Framework { get; }
    }
}