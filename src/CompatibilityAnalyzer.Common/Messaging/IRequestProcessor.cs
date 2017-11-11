using CompatibilityAnalyzer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Messaging
{
    public interface IRequestProcessor
    {
        Task ProcessAsync(AnalyzeRequest request, CancellationToken token);
    }
}
