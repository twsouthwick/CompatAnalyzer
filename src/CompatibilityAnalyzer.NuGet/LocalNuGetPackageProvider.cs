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
    public class LocalNuGetPackageProvider : INuGetPackageProvider
    {
        private readonly NuGetDownloaderSettings _settings;

        public LocalNuGetPackageProvider(NuGetDownloaderSettings settings)
        {
            _settings = settings;
        }

        public Task<IPackage> GetPackageAsync(string id, string version, CancellationToken token)
        {
            var path = Path.Combine(_settings.Feed, $"{id}.{version}.nupkg");
            var bytes = File.ReadAllBytes(path);

            return Task.FromResult<IPackage>(new NupkgPackage(id, version, bytes));
        }
    }
}