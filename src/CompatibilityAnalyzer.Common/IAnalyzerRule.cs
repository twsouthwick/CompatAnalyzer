using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public interface IAnalyzerRule
    {
        string Name { get; }

        Task RunRuleAsync(CancellationToken token);
    }
}