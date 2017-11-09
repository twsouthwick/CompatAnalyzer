using NuGet.Frameworks;
using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public interface IAssemblyCompatibilityAnalyzer
    {
        IReadOnlyCollection<Issue> Analyze(FrameworkItems version1Assemblies, FrameworkItems version2Assemblies, NuGetFramework framework);

        IEnumerable<CompatibilityRule> GetRules();
    }
}
