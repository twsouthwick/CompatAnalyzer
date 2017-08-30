using System;
using System.IO;
using Xunit;

namespace CompatibilityAnalyzer
{
    public class FileAssemblyFileTests
    {
        [Fact]
        public void FilePath()
        {
            using (var temp = new TemporaryFile())
            {
                var file = new FileAssemblyFile(temp.Path);

                Assert.Equal(temp.Path, file.Path);
            }
        }

        [Fact]
        public void FilePathToString()
        {
            using (var temp = new TemporaryFile())
            {
                var file = new FileAssemblyFile(temp.Path);

                Assert.Equal(temp.Path, file.ToString());
            }
        }

        [Fact]
        public void Contents()
        {
            using (var temp = new TemporaryFile())
            {
                var file = new FileAssemblyFile(temp.Path);

                using (var stream = file.OpenRead())
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();

                    Assert.Equal(temp.Contents, contents);
                }
            }
        }

        [Fact]
        public void InvalidFile()
        {
            Assert.Throws<FileNotFoundException>(() => new FileAssemblyFile("notafile.txt"));
        }

        [Fact]
        public void NullFile()
        {
            Assert.Throws<ArgumentNullException>("path", () => new FileAssemblyFile(null));
        }
    }
}
