namespace CompatibilityAnalyzer
{
    public interface IAnalysisOptions
    {
        PackageIdentity OriginalPackage { get; }

        PackageIdentity UpdatedPackage { get; }

        string Feed { get; }

        bool Verbose { get; }
    }
}
