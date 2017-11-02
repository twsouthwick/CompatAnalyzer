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
            var bestMatch = _reducer.GetNearest(framework, _reader.GetSupportedFrameworks());
            var libs = _reader.GetLibItems()
                .FirstOrDefault(i => i.TargetFramework == bestMatch);

            if (libs == null)
            {
                return new FrameworkItems(Enumerable.Empty<IFile>(), Enumerable.Empty<IFile>());
            }

            var items = new List<IFile>();

            foreach (var lib in libs.Items.Where(i => string.Equals(".dll", Path.GetExtension(i))))
            {
                byte[] GetBytes()
                {
                    using (var ms = new MemoryStream())
                    using (var entryStream = _reader.GetStream(lib))
                    {
                        entryStream.CopyTo(ms);

                        return ms.ToArray();
                    }
                }

                items.Add(new VersionedFile(new DelegateFile(GetBytes, lib), Version));
            }

            return new FrameworkItems(items, Enumerable.Empty<IFile>());
        }
    }
}
