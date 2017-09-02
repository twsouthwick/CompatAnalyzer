using System;
using System.IO;
using System.IO.Compression;

namespace CompatibilityAnalyzer
{
    public class ZipFileReferenceAssemblyProvider : IReferenceAssemblyProvider
    {
        private static readonly string _sourceLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "reference_assemblies.zip");
        private static readonly string _location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CompatibilityAnalyzer", "ReferenceAssemblies");

        private readonly TextWriter _writer;
        private readonly Lazy<bool> _extracted;

        public ZipFileReferenceAssemblyProvider(TextWriter writer)
        {
            _writer = writer;
            _extracted = new Lazy<bool>(Extract, true);
        }

        public string GetReferenceAssemblies(string profile)
        {
            if (!_extracted.Value)
            {
                return null;
            }

            var path = Path.Combine(_location, profile);

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(path);
            }

            return path;
        }

        private bool Extract()
        {
            Console.WriteLine("Extracting reference assemblies");

            using (var fs = File.OpenRead(_sourceLocation))
            using (var archive = new ZipArchive(fs))
            {
                archive.ExtractToDirectory(_location);
            }

            return true;
        }
    }
}
