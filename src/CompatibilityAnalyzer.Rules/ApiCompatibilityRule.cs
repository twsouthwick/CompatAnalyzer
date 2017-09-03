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

        public Task RunRuleAsync(IPackage original, IPackage updated, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
