using System;

namespace CompatibilityAnalyzer
{

    public class CompatibilityAnalysisException : Exception
    {
        public CompatibilityAnalysisException(string message)
            : base(message)
        {
        }
    }
}
