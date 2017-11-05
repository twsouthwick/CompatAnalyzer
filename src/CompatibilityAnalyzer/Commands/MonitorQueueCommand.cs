using CompatibilityAnalyzer.Messaging;
using CompatibilityAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Commands
{
    [Command(AnalysisCommand.MonitorQueue)]
    internal class MonitorQueueCommand : ICommand, IObserver<IMessage<AnalyzeRequest>>
    {
        private readonly IRequestObservable _requests;
        private readonly IEnumerable<IAnalyzerRule> _rules;
        private readonly INuGetPackageProvider _packageProvider;
        private readonly TextWriter _writer;
        private readonly TaskCompletionSource<bool> _tcs;

        public MonitorQueueCommand(IRequestObservable requests, TextWriter writer, IEnumerable<IAnalyzerRule> rules, INuGetPackageProvider packageProvider)
        {
            _requests = requests;
            _rules = rules;
            _packageProvider = packageProvider;
            _writer = writer;

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
            value.Complete();
            Console.WriteLine($"Done: {value.Message.Id}");
        }

        public async Task<IMessage<AnalyzeRequest>> OnNextAsync(IMessage<AnalyzeRequest> value)
        {
            try
            {
                Console.WriteLine($"Starting: {value.Message.Id}");

                var updatedMessage = value.Message.Updated as NuGetRequestItem;
                var originalMessage = value.Message.Original as NuGetRequestItem;

                using (var updated = await _packageProvider.GetPackageAsync(updatedMessage.Id, updatedMessage.Version, CancellationToken.None))
                using (var original = await _packageProvider.GetPackageAsync(originalMessage.Id, originalMessage.Version, CancellationToken.None))
                {
                    foreach (var rule in _rules)
                    {
                        _writer.WriteLine(rule.Name);

                        await rule.RunRuleAsync(original, updated, CancellationToken.None);
                    }
                }

                return value;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return value;
            }
        }

        public async Task RunAsync()
        {
            var observable = _requests
                .Select(x => Observable.FromAsync(() => OnNextAsync(x)))
                .Concat();

            using (observable.Subscribe(this))
            {
                await _tcs.Task;
            }
        }
    }
}
