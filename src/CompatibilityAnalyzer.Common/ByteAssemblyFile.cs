using System;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class ByteAssemblyFile : IAssemblyFile
    {
        private readonly byte[] _data;

        public ByteAssemblyFile(string path, byte[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            Path = path ?? throw new ArgumentNullException(nameof(path));

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentOutOfRangeException(nameof(path));
            }
        }

        public string Path { get; }

        public Stream OpenRead() => new MemoryStream(_data);
    }
}
