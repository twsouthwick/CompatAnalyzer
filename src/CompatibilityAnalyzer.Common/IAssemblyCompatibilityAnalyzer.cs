using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public interface IAssemblyCompatibilityAnalyzer
    {
        void Analyze(IEnumerable<IFile> version1Assemblies, IEnumerable<IFile> version2Assemblies);

        IEnumerable<string> GetRules();
    }
}
