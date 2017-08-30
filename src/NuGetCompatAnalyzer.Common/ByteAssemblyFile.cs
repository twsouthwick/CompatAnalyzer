using System.IO;

namespace NuGetCompatAnalyzer
{
    public class ByteAssemblyFile : IAssemblyFile
    {
        private readonly byte[] _data;

        public ByteAssemblyFile(string name, byte[] data)
        {
            _data = data;
            Path = name;
        }

        public string Path { get; }

        public Stream OpenReadAsync() => new MemoryStream(_data);
    }
}
