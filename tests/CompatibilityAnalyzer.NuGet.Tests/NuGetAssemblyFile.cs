using System;
using System.Collections.Generic;
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

        [MemberData(nameof(FileInstances))]
        [Theory]
        public void HashCodeGeneration(IAssemblyFile file1, IAssemblyFile file2, bool expected)
        {
            if (expected)
            {
                Assert.Equal(file1.GetHashCode(), file2.GetHashCode());
            }
            else
            {
                Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
            }
        }

        [MemberData(nameof(FileInstances))]
        [Theory]
        public void FileEquality(IAssemblyFile file1, IAssemblyFile file2, bool expected)
        {
            if (expected)
            {
                Assert.Equal(file1, file2);
            }
            else
            {
                Assert.NotEqual(file1, file2);
            }
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

        public static IEnumerable<object[]> FileInstances()
        {
            var other = new AssemblyFile(1);
            var version1 = "1.0.0";
            var version2 = "1.0.2";

            yield return new object[] { new NuGetAssemblyFile(other, version1), new NuGetAssemblyFile(other, version1), true };
            yield return new object[] { new NuGetAssemblyFile(new AssemblyFile(1), version1), new NuGetAssemblyFile(new AssemblyFile(1), version1), true };
            yield return new object[] { new NuGetAssemblyFile(new AssemblyFile(2), version1), new NuGetAssemblyFile(new AssemblyFile(1), version1), false };
            yield return new object[] { new NuGetAssemblyFile(new AssemblyFile(1), version1), new NuGetAssemblyFile(new AssemblyFile(1), version2), false };
            yield return new object[] { new NuGetAssemblyFile(new AssemblyFile(1), "0.1"), new NuGetAssemblyFile(new AssemblyFile(1), "0.1"), true };
            yield return new object[] { new NuGetAssemblyFile(new AssemblyFile(1), "0.1-Pre"), new NuGetAssemblyFile(new AssemblyFile(1), "0.1-pre"), true };
        }

        private class AssemblyFile : IAssemblyFile
        {
            private readonly int _count;

            public AssemblyFile(int count)
            {
                _count = count;
            }

            public string Path => throw new NotImplementedException();

            public Stream OpenRead() => throw new NotImplementedException();

            public override bool Equals(object obj)
            {
                if (obj is AssemblyFile other)
                {
                    return _count == other._count;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return _count;
            }
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
