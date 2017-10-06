namespace CompatibilityAnalyzer
{
    public interface IAnalysisOptions
    {
        string PackageName { get; }

        string OriginalVersion { get; }

        string UpdatedVersion { get; }

        string Feed { get; }

        bool Verbose { get; }
    }
}
