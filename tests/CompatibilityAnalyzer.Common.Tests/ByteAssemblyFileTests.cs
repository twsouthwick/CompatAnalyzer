using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace CompatibilityAnalyzer
{
    public class ByteAssemblyFileTests
    {
        [Fact]
        public void FilePath()
        {
            var path = GeneratePath();
            var file = new ByteAssemblyFile(path, Array.Empty<byte>());

            Assert.Equal(path, file.Path);
        }

        [Fact]
        public void ByteAssemblyFileToString()
        {
            var path = GeneratePath();
            var file = new ByteAssemblyFile(path, Array.Empty<byte>());

            Assert.Equal(path, file.ToString());
        }

        [Fact]
        public void Contents()
        {
            var path = GeneratePath();
            var contents = GenerateContents();

            var file = new ByteAssemblyFile(path, GetBytes(contents));

            using (var stream = file.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();

                Assert.Equal(contents, result);
            }
        }

        [Fact]
        public void InvalidPath()
        {
            Assert.Throws<ArgumentNullException>("path", () => new ByteAssemblyFile(null, Array.Empty<byte>()));
            Assert.Throws<ArgumentOutOfRangeException>("path", () => new ByteAssemblyFile(string.Empty, Array.Empty<byte>()));
        }

        [Fact]
        public void NullFile()
        {
            Assert.Throws<ArgumentNullException>("data", () => new ByteAssemblyFile(GeneratePath(), null));
        }

        [Fact]
        public void OtherTypeEquality()
        {
            var file = new ByteAssemblyFile(GeneratePath(), new byte[] { 1, 2 });

            Assert.False(file.Equals(new NoOpAssemblyFile()));
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

        public static IEnumerable<object[]> FileInstances()
        {
            var path1 = GeneratePath();
            var path2 = GeneratePath();

            var data1 = new byte[] { 1, 2, 3 };
            var data2 = Array.Empty<byte>();

            var file = new ByteAssemblyFile(GeneratePath(), new byte[] { 1, 2 });

            yield return new object[] { file, file, true };
            yield return new object[] { new ByteAssemblyFile(path1, data1), new ByteAssemblyFile(path1, data1), true };
            yield return new object[] { new ByteAssemblyFile(path1, data2), new ByteAssemblyFile(path1, data1), false };
            yield return new object[] { new ByteAssemblyFile("path", data2), new ByteAssemblyFile("path", data1), false };

            // Check case sensitivies
            yield return new object[] { new ByteAssemblyFile("path", data2), new ByteAssemblyFile("path", data2), true };
            yield return new object[] { new ByteAssemblyFile("Path", data2), new ByteAssemblyFile("path", data2), false };
        }

        private static string GeneratePath() => Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        private static string GenerateContents() => Guid.NewGuid().ToString();

        private static byte[] GetBytes(string contents) => Encoding.UTF8.GetBytes(contents);

        private class NoOpAssemblyFile : IAssemblyFile
        {
            public string Path => throw new NotImplementedException();

            public Stream OpenRead() => throw new NotImplementedException();
        }
    }
}
