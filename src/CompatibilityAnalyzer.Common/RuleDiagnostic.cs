namespace CompatibilityAnalyzer
{
    public class RuleDiagnostic
    {
        public RuleDiagnostic(string id, string message)
        {
            Id = id;
            Message = message;
        }

        public string Id { get; }

        public string Message { get; }
    }
}
