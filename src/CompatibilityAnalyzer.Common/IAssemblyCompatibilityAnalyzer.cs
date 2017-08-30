using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public interface IAssemblyCompatibilityAnalyzer
    {
        void Analyze(IEnumerable<IAssemblyFile> version1Assemblies, IEnumerable<IAssemblyFile> version2Assemblies);

        IEnumerable<string> GetRules();
    }
}
