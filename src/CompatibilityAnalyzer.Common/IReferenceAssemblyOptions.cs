namespace CompatibilityAnalyzer
{
    public interface IReferenceAssemblyOptions
    {
        bool Create { get; }

        string ReferencePath { get; }
    }
}
