using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public class ApiCompatibilityRule : IAnalyzerRule
    {
        private readonly TextWriter _writer;
        private readonly IAssemblyCompatibilityAnalyzer _analyzer;

        public ApiCompatibilityRule(TextWriter writer, IAssemblyCompatibilityAnalyzer analyzer)
        {
            _writer = writer;
            _analyzer = analyzer;
        }

        public string Name => "API Compatibility";

        public Task<IReadOnlyCollection<RuleDiagnostic>> RunRuleAsync(IPackage original, IPackage updated, CancellationToken token)
        {
            var result = new List<RuleDiagnostic>();

            foreach (var framework in updated.SupportedFrameworks)
            {
                if (framework.IsAny)
                {
                    continue;
                }

                _writer.WriteLine($"[{framework}] Started");

                try
                {
                    var updatedAssemblies = updated.GetAssemblies(framework);
                    var originalAssemblies = original.GetAssemblies(framework);

                    var results = _analyzer.Analyze(originalAssemblies, updatedAssemblies, framework);

                    _writer.WriteLine($"[{framework}] Found {results.Count} issues");
                }
                catch (CompatibilityAnalysisException e)
                {
                    _writer.WriteLine(e.Message);
                }
                finally
                {
                    _writer.WriteLine($"[{framework}] Finished");
                }
            }

            return Task.FromResult<IReadOnlyCollection<RuleDiagnostic>>(result);
        }
    }
}
