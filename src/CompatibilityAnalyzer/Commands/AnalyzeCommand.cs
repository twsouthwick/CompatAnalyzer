using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    [Command(AnalysisCommand.Analyze)]
    internal class AnalyzeCommand : ICommand
    {
        private readonly IEnumerable<IAnalyzerRule> _rules;
        private readonly INuGetPackageProvider _packageProvider;
        private readonly IAnalysisOptions _options;
        private readonly TextWriter _writer;

        public AnalyzeCommand(IEnumerable<IAnalyzerRule> rules, TextWriter writer, INuGetPackageProvider packageProvider, IAnalysisOptions options)
        {
            _rules = rules;
            _writer = writer;
            _packageProvider = packageProvider;
            _options = options;
        }

        public async Task RunAsync()
        {
            using (var updated = await _packageProvider.GetPackageAsync(_options.PackageName, _options.UpdatedVersion, CancellationToken.None))
            using (var original = await _packageProvider.GetPackageAsync(_options.PackageName, _options.OriginalVersion, CancellationToken.None))
            {
                foreach (var rule in _rules)
                {
                    _writer.WriteLine(rule.Name);

                    await rule.RunRuleAsync(original, updated, CancellationToken.None);
                }
            }
        }
    }
}
