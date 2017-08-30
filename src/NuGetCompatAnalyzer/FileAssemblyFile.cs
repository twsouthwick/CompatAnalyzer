using System.IO;

namespace NuGetCompatAnalyzer
{
    internal class FileAssemblyFile : ApiCompat.IAssemblyFile
    {
        public FileAssemblyFile(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public Stream OpenReadAsync() => File.OpenRead(Path);
    }
}
