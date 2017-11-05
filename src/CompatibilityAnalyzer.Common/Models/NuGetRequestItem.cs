using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Models
{
    public class NuGetRequestItem : IRequestItem
    {
        public string Feed { get; set; }

        public string Id { get; set; }

        public string Version { get; set; }

        public Task<IPackage> GetPackageAsync(INuGetPackageProvider provider, CancellationToken token)
        {
            return provider.GetPackageAsync(Id, Version, token);
        }
    }
}
