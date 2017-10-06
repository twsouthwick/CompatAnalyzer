using System;

namespace CompatibilityAnalyzer
{
    internal class CommandAttribute : Attribute
    {
        public CommandAttribute(AnalysisCommand command)
        {
            Command = command;
        }

        public AnalysisCommand Command { get; }
    }
}
