using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Messaging
{
    public interface IQueueListener
    {
        Task ProcessQueueAsync(CancellationToken token);
    }
}
