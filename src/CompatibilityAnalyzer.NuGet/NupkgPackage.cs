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
            SupportedFrameworks = _reader.GetSupportedFrameworks()
                .Select(f => new FrameworkInfo(f.Framework))
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
            var nugetFramework = NuGetFramework.Parse(framework.Framework);
            var libs = _reader.GetLibItems()
                .FirstOrDefault(i => i.TargetFramework == nugetFramework);

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
