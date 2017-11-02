using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public interface IAssemblyCompatibilityAnalyzer
    {
        void Analyze(FrameworkItems version1Assemblies, FrameworkItems version2Assemblies, FrameworkInfo framework);

        IEnumerable<CompatibilityRule> GetRules();
    }
}
