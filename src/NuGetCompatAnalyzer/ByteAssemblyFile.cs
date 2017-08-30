using System.IO;

namespace NuGetCompatAnalyzer
{
    internal class ByteAssemblyFile : ApiCompat.IAssemblyFile
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
