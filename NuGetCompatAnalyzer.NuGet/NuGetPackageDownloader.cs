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
    public class NuGetPackageDownloader : IDisposable
    {
        private readonly HttpClientHandler _handler;
        private readonly HttpHandlerResourceV3 _httpResource;
        private readonly SourceRepository _repository;

        public NuGetPackageDownloader(string feed)
        {
            _handler = new HttpClientHandler();
            _httpResource = new HttpHandlerResourceV3(_handler, _handler);
            _repository = Repository.Factory.GetCoreV3(feed);
        }

        public void Dispose()
        {
            _handler.Dispose();
        }

        public async Task<NupkgData> DownloadAsync(string id, string version, CancellationToken token)
        {
            var finder = await _repository.GetResourceAsync<FindPackageByIdResource>();
            var nugetVersion = NuGetVersion.Parse(version);

            using (var ms = new MemoryStream())
            {
                if (await finder.CopyNupkgToStreamAsync(id, nugetVersion, ms, new SourceCacheContext(), NullLogger.Instance, token))
                {
                    return new NupkgData(id, version, ms.ToArray());
                }

                throw new InvalidOperationException("Could not copy nupkg");
            }
        }
    }
}
