using System;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class NuGetAssemblyFile : IAssemblyFile
    {
        private readonly IAssemblyFile _other;

        public NuGetAssemblyFile(IAssemblyFile other, string version)
        {
            _other = other ?? throw new ArgumentNullException(nameof(other));
            Version = version ?? throw new ArgumentNullException(nameof(version));

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentOutOfRangeException(nameof(version));
            }
        }

        public string Path => _other.Path;

        public string Version { get; }

        public Stream OpenRead() => _other.OpenRead();

        public override string ToString() => $"{Path} [{Version}]";

        public override bool Equals(object obj)
        {
            if (obj is NuGetAssemblyFile other)
            {
                return string.Equals(Version, other.Version, StringComparison.OrdinalIgnoreCase)
                    && Equals(other._other, _other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _other.GetHashCode() ^ StringComparer.OrdinalIgnoreCase.GetHashCode(Version);
        }
    }
}
