using System;
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

        private static string GeneratePath() => Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        private static string GenerateContents() => Guid.NewGuid().ToString();

        private static byte[] GetBytes(string contents) => Encoding.UTF8.GetBytes(contents);
    }
}
