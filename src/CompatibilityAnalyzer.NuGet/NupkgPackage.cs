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
        private readonly FrameworkReducer _reducer;

        public NupkgPackage(string id, string version, byte[] data)
        {
            Id = id;
            Version = version;

            _provider = CompatibilityListProvider.Default;
            _reader = new PackageArchiveReader(new MemoryStream(data));
            _reducer = new FrameworkReducer();

            SupportedFrameworks = _reader.GetSupportedFrameworks()
                .SelectMany(_provider.GetFrameworksSupporting)
                .Distinct()
                .OrderBy(f => f.Framework, StringComparer.Ordinal)
                .Select(f => new FrameworkInfo(f.Framework, f.Version))
                .ToList();
        }

        public string Id { get; }

        public string Version { get; }

        public IReadOnlyCollection<FrameworkInfo> SupportedFrameworks { get; }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public IEnumerable<IFile> GetAssemblies(FrameworkInfo framework)
        {
            var nugetFramework = new NuGetFramework(framework.Framework, framework.Version);
            var bestMatch = _reducer.GetNearest(nugetFramework, _reader.GetSupportedFrameworks());
            var libs = _reader.GetLibItems()
                .FirstOrDefault(i => i.TargetFramework == bestMatch);

            if (libs == null)
            {
                yield break;
            }

            foreach (var lib in libs.Items.Where(i => string.Equals(".dll", Path.GetExtension(i))))
            {
                using (var ms = new MemoryStream())
                using (var entryStream = _reader.GetStream(lib))
                {
                    entryStream.CopyTo(ms);

                    yield return new VersionedFile(new ByteFile(lib, ms.ToArray()), Version);
                }
            }
        }
    }
}
