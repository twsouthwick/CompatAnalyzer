using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CompatibilityAnalyzer
{
    public class FileSystemFileTests
    {
        [Fact]
        public void FilePath()
        {
            using (var temp = new TemporaryFile())
            {
                var file = new FileSystemFile(temp.Path);

                Assert.Equal(temp.Path, file.Path);
            }
        }

        [Fact]
        public void FilePathToString()
        {
            using (var temp = new TemporaryFile())
            {
                var file = new FileSystemFile(temp.Path);

                Assert.Equal(temp.Path, file.ToString());
            }
        }

        [Fact]
        public void Contents()
        {
            using (var temp = new TemporaryFile())
            {
                var file = new FileSystemFile(temp.Path);

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
            Assert.Throws<FileNotFoundException>(() => new FileSystemFile("notafile.txt"));
        }

        [Fact]
        public void NullFile()
        {
            Assert.Throws<ArgumentNullException>("path", () => new FileSystemFile(null));
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
            if(expected)
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
            var guid = Guid.NewGuid().ToString();

            using (var temp1 = new TemporaryFile())
            using (var temp2 = new TemporaryFile())
            using (var temp3_lower = new TemporaryFile(Path.Combine(Path.GetTempPath(), $"{guid}-something.Txt")))
            using (var temp3_upper = new TemporaryFile(Path.Combine(Path.GetTempPath(), $"{guid}-Something.Txt")))
            {
                var file = new FileSystemFile(temp1.Path);

                yield return new object[] { file, file, true };
                yield return new object[] { new FileSystemFile(temp1.Path), new FileSystemFile(temp1.Path), true };
                yield return new object[] { new FileSystemFile(temp1.Path), new FileSystemFile(temp2.Path), false };
                yield return new object[] { new FileSystemFile(temp3_lower.Path), new FileSystemFile(temp3_upper.Path), true };
            }
        }
    }
}
