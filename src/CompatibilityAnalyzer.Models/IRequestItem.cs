using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Models
{
    public interface IRequestItem
    {
        Task<IPackage> GetPackageAsync(INuGetPackageProvider provider, CancellationToken token);
    }
}
