using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CompatibilityAnalyzer
{
    public class VersionedFileTests
    {
        [Fact]
        public void NullOtherAssemblyFile()
        {
            Assert.Throws<ArgumentNullException>("other", () => new VersionedFile(null, "1.0.0"));
        }

        [Fact]
        public void NullVersion()
        {
            Assert.Throws<ArgumentNullException>("version", () => new VersionedFile(new PathOnlyAssemblyFile(), null));
        }

        [Fact]
        public void EmptyStringVersion()
        {
            Assert.Throws<ArgumentOutOfRangeException>("version", () => new VersionedFile(new PathOnlyAssemblyFile(), string.Empty));
        }

        [Fact]
        public void PathIsCorrect()
        {
            var pathOnly = new PathOnlyAssemblyFile();
            var nugetFile = new VersionedFile(pathOnly, "1.0.0");

            Assert.Equal(pathOnly.Path, nugetFile.Path);
        }

        [Fact]
        public void FileIsSame()
        {
            var pathOnly = new PathOnlyAssemblyFile();
            var nugetFile = new VersionedFile(pathOnly, "1.0.0");

            Assert.Same(pathOnly.OpenRead(), nugetFile.OpenRead());
        }

        [MemberData(nameof(FileInstances))]
        [Theory]
        public void HashCodeGeneration(IFile file1, IFile file2, bool expected)
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
        public void FileEquality(IFile file1, IFile file2, bool expected)
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
            var nugetFile = new VersionedFile(pathOnly, version);

            Assert.Equal(version, nugetFile.Version);
        }

        [Fact]
        public void VersionToString()
        {
            var version = "1.0.0";
            var pathOnly = new PathOnlyAssemblyFile();
            var nugetFile = new VersionedFile(pathOnly, version);

            Assert.Equal($"{pathOnly.Path} [{version}]", nugetFile.ToString());
        }

        public static IEnumerable<object[]> FileInstances()
        {
            var other = new AssemblyFile(1);
            var version1 = "1.0.0";
            var version2 = "1.0.2";

            yield return new object[] { new VersionedFile(other, version1), new VersionedFile(other, version1), true };
            yield return new object[] { new VersionedFile(new AssemblyFile(1), version1), new VersionedFile(new AssemblyFile(1), version1), true };
            yield return new object[] { new VersionedFile(new AssemblyFile(2), version1), new VersionedFile(new AssemblyFile(1), version1), false };
            yield return new object[] { new VersionedFile(new AssemblyFile(1), version1), new VersionedFile(new AssemblyFile(1), version2), false };
            yield return new object[] { new VersionedFile(new AssemblyFile(1), "0.1"), new VersionedFile(new AssemblyFile(1), "0.1"), true };
            yield return new object[] { new VersionedFile(new AssemblyFile(1), "0.1-Pre"), new VersionedFile(new AssemblyFile(1), "0.1-pre"), true };
        }

        private class AssemblyFile : IFile
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

        private class PathOnlyAssemblyFile : IFile
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
