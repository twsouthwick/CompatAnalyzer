using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public interface IReferenceAssemblyProvider
    {
        IEnumerable<string> SupportedProfiles { get; }

        IEnumerable<string> GetReferenceAssemblyPath(string profile);
    }
}
