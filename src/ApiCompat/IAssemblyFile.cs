using System.IO;

namespace ApiCompat
{
    public interface IAssemblyFile
    {
        string Path { get; }

        Stream OpenReadAsync();
    }
}
