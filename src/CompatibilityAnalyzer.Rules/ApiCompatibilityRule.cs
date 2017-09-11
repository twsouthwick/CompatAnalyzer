using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public class ApiCompatibilityRule : IAnalyzerRule
    {
        private readonly IAssemblyCompatibilityAnalyzer _analyzer;

        public ApiCompatibilityRule(IAssemblyCompatibilityAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        public string Name => "API Compatibility";

        public Task<IReadOnlyCollection<RuleDiagnostic>> RunRuleAsync(IPackage original, IPackage updated, CancellationToken token)
        {
            var result = new List<RuleDiagnostic>();

            foreach (var framework in updated.SupportedFrameworks)
            {
                var updatedAssemblies = updated.GetAssemblies(framework);
                var originalAssemblies = original.GetAssemblies(framework);

                _analyzer.Analyze(originalAssemblies, updatedAssemblies);
            }

            return Task.FromResult<IReadOnlyCollection<RuleDiagnostic>>(result);
        }
    }
}
