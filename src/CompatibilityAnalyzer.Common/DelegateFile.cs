using System;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class DelegateFile : IFile
    {
        private readonly Lazy<byte[]> _factory;

        public DelegateFile(Func<byte[]> factory, string path)
        {
            _factory = new Lazy<byte[]>(factory, true);

            Path = path;
        }

        public string Path { get; }

        public Stream OpenRead() => new MemoryStream(_factory.Value);

        public override string ToString() => Path;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is DelegateFile del)
            {
                return string.Equals(Path, del.Path, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Path);
        }
    }
}
