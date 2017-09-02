using NuGet.Frameworks;
using NuGet.Packaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompatibilityAnalyzer
{
    public class NupkgPackage : IPackage
    {
        private readonly PackageArchiveReader _reader;

        public NupkgPackage(string id, string version, byte[] data)
        {
            Id = id;
            Version = version;

            _reader = new PackageArchiveReader(new MemoryStream(data));
            Frameworks = _reader.GetSupportedFrameworks()
                .Select(f => new WrappedNuGetFramework(f))
                .ToList();
        }

        public string Id { get; }

        public string Version { get; }

        public IReadOnlyCollection<INuGetFramework> Frameworks { get; }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public IEnumerable<IFile> GetAssemblies(INuGetFramework framework)
        {
            var libs = _reader.GetLibItems()
                .FirstOrDefault(i => i.TargetFramework == framework.AsFramework());

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
