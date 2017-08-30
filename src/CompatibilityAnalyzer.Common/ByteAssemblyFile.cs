using System;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class ByteAssemblyFile : IAssemblyFile
    {
        public ByteAssemblyFile(string path, byte[] data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Path = path ?? throw new ArgumentNullException(nameof(path));

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentOutOfRangeException(nameof(path));
            }
        }

        public byte[] Data { get; }

        public string Path { get; }

        public Stream OpenRead() => new MemoryStream(Data);

        public override string ToString() => Path;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is ByteAssemblyFile other)
            {
                return ReferenceEquals(Data, other.Data)
                    && string.Equals(Path, other.Path, StringComparison.Ordinal);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Path) ^ Data.GetHashCode();
        }
    }
}
