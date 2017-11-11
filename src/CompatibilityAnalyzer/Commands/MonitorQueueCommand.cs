using CompatibilityAnalyzer.Messaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Commands
{
    [Command(AnalysisCommand.MonitorQueue)]
    internal class MonitorQueueCommand : ICommand
    {
        private readonly IQueueListener _listener;
        private readonly TextWriter _writer;

        public MonitorQueueCommand(IQueueListener listener, TextWriter writer)
        {
            _listener = listener;
            _writer = writer;
        }

        public Task RunAsync() => _listener.ProcessQueueAsync(CancellationToken.None);
    }
}
