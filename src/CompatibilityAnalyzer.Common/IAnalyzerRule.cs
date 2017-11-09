using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public interface IAnalyzerRule
    {
        string Name { get; }

        Task<IReadOnlyCollection<Issue>> RunRuleAsync(IPackage original, IPackage updated, CancellationToken token);
    }
}
