using System.IO;

namespace NuGetCompatAnalyzer
{
    public class FileAssemblyFile : IAssemblyFile
    {
        public FileAssemblyFile(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public Stream OpenReadAsync() => File.OpenRead(Path);
    }
}
