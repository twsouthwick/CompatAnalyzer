using System;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class FileAssemblyFile : IAssemblyFile
    {
        public FileAssemblyFile(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));

            if (!File.Exists(Path))
            {
                throw new FileNotFoundException("Could not find file", path);
            }
        }

        public string Path { get; }

        public Stream OpenRead() => File.OpenRead(Path);
    }
}
