using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CompatibilityAnalyzer
{
    public class ZipFileReferenceAssemblyProvider : IReferenceAssemblyProvider
    {
        private readonly TextWriter _writer;
        private readonly IReferenceAssemblyOptions _options;
        private readonly Lazy<bool> _extracted;
        private readonly string _location;

        public ZipFileReferenceAssemblyProvider(TextWriter writer, IReferenceAssemblyOptions options)
        {
            _writer = writer;
            _options = options;
            _extracted = new Lazy<bool>(Extract, true);
            _location = Path.Combine(Path.GetTempPath(), "CompatibilityAnalyzer", "ReferenceAssemblies", Guid.NewGuid().ToString());
        }

        public IEnumerable<string> SupportedProfiles => Directory.EnumerateDirectories(_location).Select(Path.GetFileName);

        public IEnumerable<string> GetReferenceAssemblyPath(string profile)
        {
            if (!_extracted.Value)
            {
                yield break;
            }

            var path = Path.Combine(_location, profile);

            if (!Directory.Exists(path))
            {
                throw new ProfileNotFound(profile);
            }

            yield return path;
        }

        private bool Extract()
        {
            _writer.WriteLine("Extracting reference assemblies");

            using (var fs = File.OpenRead(_options.ReferencePath))
            using (var archive = new ZipArchive(fs))
            {
                archive.ExtractToDirectory(_location);
            }

            return true;
        }
    }
}
