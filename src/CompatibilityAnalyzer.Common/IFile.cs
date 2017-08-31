using System.IO;

namespace CompatibilityAnalyzer
{
    public interface IFile
    {
        string Path { get; }

        Stream OpenRead();
    }
}
