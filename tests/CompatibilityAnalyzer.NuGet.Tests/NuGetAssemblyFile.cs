using System;
using System.IO;
using Xunit;

namespace CompatibilityAnalyzer
{
    public class NuGetAssemblyFileTests
    {
        [Fact]
        public void NullOtherAssemblyFile()
        {
            Assert.Throws<ArgumentNullException>("other", () => new NuGetAssemblyFile(null, "1.0.0"));
        }

        [Fact]
        public void NullVersion()
        {
            Assert.Throws<ArgumentNullException>("version", () => new NuGetAssemblyFile(new PathOnlyAssemblyFile(), null));
        }

        [Fact]
        public void EmptyStringVersion()
        {
            Assert.Throws<ArgumentOutOfRangeException>("version", () => new NuGetAssemblyFile(new PathOnlyAssemblyFile(), string.Empty));
        }

        [Fact]
        public void PathIsCorrect()
        {
            var pathOnly = new PathOnlyAssemblyFile();
            var nugetFile = new NuGetAssemblyFile(pathOnly, "1.0.0");

            Assert.Equal(pathOnly.Path, nugetFile.Path);
        }

        [Fact]
        public void FileIsSame()
        {
            var pathOnly = new PathOnlyAssemblyFile();
            var nugetFile = new NuGetAssemblyFile(pathOnly, "1.0.0");

            Assert.Same(pathOnly.OpenRead(), nugetFile.OpenRead());
        }

        [Fact]
        public void VersionIsSet()
        {
            var version = "1.0.0";
            var pathOnly = new PathOnlyAssemblyFile();
            var nugetFile = new NuGetAssemblyFile(pathOnly, version);

            Assert.Equal(version, nugetFile.Version);
        }

        [Fact]
        public void VersionToString()
        {
            var version = "1.0.0";
            var pathOnly = new PathOnlyAssemblyFile();
            var nugetFile = new NuGetAssemblyFile(pathOnly, version);

            Assert.Equal($"{pathOnly.Path} [{version}]", nugetFile.ToString());
        }

        private class PathOnlyAssemblyFile : IAssemblyFile
        {
            private readonly Stream _stream;

            public PathOnlyAssemblyFile()
            {
                Path = Guid.NewGuid().ToString();
                _stream = new MemoryStream();
            }

            public string Path { get; }

            public Stream OpenRead() => _stream;
        }
    }
}
