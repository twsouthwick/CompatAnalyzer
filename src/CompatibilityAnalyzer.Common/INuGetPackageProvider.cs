using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public interface INuGetPackageProvider
    {
        Task<IPackage> GetPackageAsync(string id, string version, CancellationToken token);
    }
}
