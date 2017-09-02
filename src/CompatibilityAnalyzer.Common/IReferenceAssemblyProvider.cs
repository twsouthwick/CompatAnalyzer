using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public interface IReferenceAssemblyProvider
    {
        string GetReferenceAssemblies(string profile);
    }
}
