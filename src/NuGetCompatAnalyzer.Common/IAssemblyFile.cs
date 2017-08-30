using System.IO;

namespace NuGetCompatAnalyzer
{
    public interface IAssemblyFile
    {
        string Path { get; }

        Stream OpenReadAsync();
    }
}
