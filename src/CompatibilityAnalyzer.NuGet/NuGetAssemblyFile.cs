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
    }
}
