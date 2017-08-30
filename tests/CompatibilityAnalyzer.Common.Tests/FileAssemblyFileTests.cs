using System;
using System.Collections.Generic;
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

        [Fact]
        public void HashCodeGeneration()
        {
            using (var temp1 = new TemporaryFile())
            using (var temp2 = new TemporaryFile())
            {
                var file1 = new FileAssemblyFile(temp1.Path);
                var file1b = new FileAssemblyFile(temp1.Path);
                var file2 = new FileAssemblyFile(temp2.Path);

                Assert.Equal(file1.GetHashCode(), file1b.GetHashCode());
                Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
                Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
                Assert.NotEqual(file1.GetHashCode(), file2.GetHashCode());
            }
        }

        [Fact]
        public void HashCodeGenerationCaseInsensitive()
        {
            var guid = Guid.NewGuid().ToString();

            using (var temp_lower = new TemporaryFile(Path.Combine(Path.GetTempPath(), $"{guid}-something.Txt")))
            using (var temp_upper = new TemporaryFile(Path.Combine(Path.GetTempPath(), $"{guid}-Something.Txt")))
            {
                var lower = new FileAssemblyFile(temp_lower.Path);
                var upper = new FileAssemblyFile(temp_upper.Path);

                Assert.Equal(lower.GetHashCode(), upper.GetHashCode());
            }
        }

        [MemberData(nameof(FileInstances))]
        [Theory]
        public void FileEquality(IAssemblyFile file1, IAssemblyFile file2, bool expected)
        {
            Assert.Equal(expected, Equals(file1, file2));
        }

        public static IEnumerable<object[]> FileInstances()
        {
            var guid = Guid.NewGuid().ToString();

            using (var temp1 = new TemporaryFile())
            using (var temp2 = new TemporaryFile())
            using (var temp3_lower = new TemporaryFile(Path.Combine(Path.GetTempPath(), $"{guid}-something.Txt")))
            using (var temp3_upper = new TemporaryFile(Path.Combine(Path.GetTempPath(), $"{guid}-Something.Txt")))
            {
                var file = new FileAssemblyFile(temp1.Path);

                yield return new object[] { file, file, true };
                yield return new object[] { new FileAssemblyFile(temp1.Path), new FileAssemblyFile(temp1.Path), true };
                yield return new object[] { new FileAssemblyFile(temp1.Path), new FileAssemblyFile(temp2.Path), false };
                yield return new object[] { new FileAssemblyFile(temp3_lower.Path), new FileAssemblyFile(temp3_upper.Path), true };
            }
        }
    }
}
