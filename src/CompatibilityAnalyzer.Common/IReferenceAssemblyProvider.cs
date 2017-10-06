using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public interface IReferenceAssemblyProvider
    {
        IEnumerable<string> SupportedProfiles { get; }

        string GetReferenceAssemblyPath(string profile);
    }
}
