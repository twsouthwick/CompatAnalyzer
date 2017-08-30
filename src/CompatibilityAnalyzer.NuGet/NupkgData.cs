using NuGet.Frameworks;
using NuGet.Packaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompatibilityAnalyzer
{
    public class NupkgData
    {
        private readonly byte[] _data;

        public NupkgData(string id, string version, byte[] data)
        {
            _data = data;
            Id = id;
            Version = version;
        }

        public string Id { get; }

        public string Version { get; }

        public IReadOnlyCollection<NuGetFramework> GetFrameworks()
        {
            using (var stream = new MemoryStream(_data))
            using (var reader = new PackageArchiveReader(stream))
            {
                return reader.GetSupportedFrameworks()
                    .ToList();
            }
        }

        public IEnumerable<IAssemblyFile> GetAssemblies(NuGetFramework framework)
        {
            using (var stream = new MemoryStream(_data))
            using (var reader = new PackageArchiveReader(stream))
            {
                var libs = reader.GetLibItems()
                    .FirstOrDefault(i => i.TargetFramework == framework);

                if (libs == null)
                {
                    yield break;
                }

                foreach (var lib in libs.Items.Where(i => string.Equals(".dll", Path.GetExtension(i))))
                {
                    using (var ms = new MemoryStream())
                    using (var entryStream = reader.GetStream(lib))
                    {
                        entryStream.CopyTo(ms);

                        yield return new ByteAssemblyFile(lib, ms.ToArray());
                    }
                }
            }
        }
    }
}
