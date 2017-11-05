using CompatibilityAnalyzer.Messaging;
using CompatibilityAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Commands
{
    [Command(AnalysisCommand.MonitorQueue)]
    internal class MonitorQueueCommand : ICommand, IObserver<IMessage<AnalyzeRequest>>
    {
        private readonly IRequestObservable _requests;
        private readonly TaskCompletionSource<bool> _tcs;

        public MonitorQueueCommand(IRequestObservable requests)
        {
            _requests = requests;
            _tcs = new TaskCompletionSource<bool>();
        }

        public void OnCompleted()
        {
            Console.WriteLine("Completed");
            _tcs.SetResult(true);
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"Error: {error}");
            _tcs.SetResult(false);
        }

        public void OnNext(IMessage<AnalyzeRequest> value)
        {
            Console.WriteLine($"ID: {value.Message.Id}");

            value.Complete();
        }

        public async Task RunAsync()
        {
            using (_requests.Subscribe(this))
            {
                await _tcs.Task;
            }
        }
    }
}
