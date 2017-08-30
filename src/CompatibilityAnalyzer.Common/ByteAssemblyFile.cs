using System;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class ByteAssemblyFile : IAssemblyFile
    {
        private readonly byte[] _data;

        public ByteAssemblyFile(string path, byte[] data)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentOutOfRangeException(nameof(path));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _data = data;
            Path = path;
        }

        public string Path { get; }

        public Stream OpenRead() => new MemoryStream(_data);
    }
}
