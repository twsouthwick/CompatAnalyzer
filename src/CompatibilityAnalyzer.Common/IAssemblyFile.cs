using System.IO;

namespace CompatibilityAnalyzer
{
    public interface IAssemblyFile
    {
        string Path { get; }

        Stream OpenRead();
    }
}
