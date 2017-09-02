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
    public interface INuGetPackageProvider
    {
        Task<IPackage> GetPackageAsync(string id, string version, CancellationToken token);
    }
}