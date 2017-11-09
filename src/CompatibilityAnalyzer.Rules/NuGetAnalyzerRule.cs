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

        public Task<IReadOnlyCollection<Issue>> RunRuleAsync(IPackage original, IPackage updated, CancellationToken token)
        {
            return Task.FromResult<IReadOnlyCollection<Issue>>(Array.Empty<Issue>());
        }
    }
}
