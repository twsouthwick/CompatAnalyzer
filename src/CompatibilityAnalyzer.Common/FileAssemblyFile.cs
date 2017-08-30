using System;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class FileAssemblyFile : IAssemblyFile
    {
        public FileAssemblyFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Could not find file", path);
            }

            Path = path;
        }

        public string Path { get; }

        public Stream OpenRead() => File.OpenRead(Path);
    }
}
