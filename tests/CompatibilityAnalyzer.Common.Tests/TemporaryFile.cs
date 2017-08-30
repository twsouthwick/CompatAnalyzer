using System;
using System.IO;

namespace CompatibilityAnalyzer
{
    internal class TemporaryFile : IDisposable
    {
        public TemporaryFile()
            : this(System.IO.Path.GetTempFileName())
        {
        }

        public TemporaryFile(string path)
            : this(path, Guid.NewGuid().ToString())
        {
        }

        public TemporaryFile(string path, string contents)
        {
            Path = path;
            Contents = contents;

            File.WriteAllText(Path, Contents);
        }

        public string Path { get; }

        public string Contents { get; }

        public void Dispose()
        {
            File.Delete(Path);
        }
    }
}
