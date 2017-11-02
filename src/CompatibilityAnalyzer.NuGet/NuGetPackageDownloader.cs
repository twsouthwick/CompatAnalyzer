using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public class NuGetPackageDownloader : INuGetPackageProvider
    {
        private readonly SourceRepository _repository;
        private readonly SourceCacheContext _cache;
        private readonly ILogger _log;

        public NuGetPackageDownloader(NuGetDownloaderSettings settings, SourceRepository repository, SourceCacheContext cache, ILogger log)
        {
            _repository = repository;
            _cache = cache;
            _log = log;
        }

        public async Task<IPackage> GetPackageAsync(string id, string version, CancellationToken token)
        {
            var finder = await _repository.GetResourceAsync<FindPackageByIdResource>();
            var nugetVersion = NuGetVersion.Parse(version);

            using (var ms = new MemoryStream())
            {
                if (await finder.CopyNupkgToStreamAsync(id, nugetVersion, ms, _cache, _log, token))
                {
                    return new NupkgPackage(id, version, ms.ToArray());
                }

                throw new InvalidOperationException("Could not copy nupkg");
            }
        }
    }
}
