using NuGet.Frameworks;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompatibilityAnalyzer
{
    public class NupkgPackage : IPackage
    {
        private readonly PackageArchiveReader _reader;
        private readonly IFrameworkCompatibilityListProvider _provider;

        public NupkgPackage(string id, string version, byte[] data)
        {
            Id = id;
            Version = version;

            _provider = CompatibilityListProvider.Default;
            _reader = new PackageArchiveReader(new MemoryStream(data));

            SupportedFrameworks = _reader.GetSupportedFrameworks()
                .ToList();
        }

        public string Id { get; }

        public string Version { get; }

        public IReadOnlyCollection<NuGetFramework> SupportedFrameworks { get; }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public FrameworkItems GetAssemblies(NuGetFramework framework)
        {
            if (_dependencies.TryGetValue(framework, out var files))
            {
                return new FrameworkItems(_reader.GetAssemblies(framework).ToList(), files);
            }
            else
            {
                return new FrameworkItems(_reader.GetAssemblies(framework).ToList(), Enumerable.Empty<IFile>());
            }
        }
    }
}
