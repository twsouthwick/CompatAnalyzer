using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public class NuGetAnalyzerRule : IAnalyzerRule
    {
        private readonly TextWriter _writer;

        public NuGetAnalyzerRule(TextWriter writer)
        {
            _writer = writer;
        }

        public string Name => "NuGet target analyzer";

        public Task<IReadOnlyCollection<RuleDiagnostic>> RunRuleAsync(IPackage original, IPackage updated, CancellationToken token)
        {
            Print("original", original);
            Print("updated", updated);

            return Task.FromResult<IReadOnlyCollection<RuleDiagnostic>>(Array.Empty<RuleDiagnostic>());
        }

        private void Print(string title, IPackage package)
        {
            Console.WriteLine(title);

            foreach (var framework in package.SupportedFrameworks)
            {
                _writer.Write("\t");
                _writer.WriteLine(framework);
            }
        }
    }
}
