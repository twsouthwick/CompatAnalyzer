using System;
using System.IO;

namespace CompatibilityAnalyzer
{
    internal class TemporaryFile : IDisposable
    {
        public TemporaryFile()
            : this(Guid.NewGuid().ToString())
        {
        }

        public TemporaryFile(string contents)
        {
            Path = System.IO.Path.GetTempFileName();
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
