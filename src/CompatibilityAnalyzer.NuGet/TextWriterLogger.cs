using NuGet.Common;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class TextWriterLogger : ILogger
    {
        private readonly TextWriter _writer;

        public TextWriterLogger(TextWriter writer)
        {
            _writer = writer;
        }

        public void LogDebug(string data) => _writer.WriteLine($"[Debug] {data}");

        public void LogError(string data) => _writer.WriteLine($"[Error] {data}");

        public void LogErrorSummary(string data) => _writer.WriteLine($"[Error Summary] {data}");

        public void LogInformation(string data) => _writer.WriteLine($"[Info] {data}");

        public void LogInformationSummary(string data) => _writer.WriteLine($"[Info Summary] {data}");

        public void LogMinimal(string data) => _writer.WriteLine($"[Minimal] {data}");

        public void LogVerbose(string data) => _writer.WriteLine($"[Verbose] {data}");

        public void LogWarning(string data) => _writer.WriteLine($"[Warning] {data}");
    }
}
