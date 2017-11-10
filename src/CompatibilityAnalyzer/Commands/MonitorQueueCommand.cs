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
        private readonly IResultStorage _storage;
        private readonly TextWriter _writer;
        private readonly TaskCompletionSource<bool> _tcs;

        public MonitorQueueCommand(IRequestObservable requests, TextWriter writer, IEnumerable<IAnalyzerRule> rules, INuGetPackageProvider packageProvider, IResultStorage storage)
        {
            _requests = requests;
            _rules = rules;
            _packageProvider = packageProvider;
            _writer = writer;
            _storage = storage;

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
                var finalResults = new List<Issue>();

                Console.WriteLine($"Starting: {value.Message.Id}");

                await _storage.UpdateAsync(new IssueResults
                {
                    Id = value.Message.Id,
                    Issues = Array.Empty<Issue>(),
                    State = IssueResultState.Processing
                }, CancellationToken.None);

                using (var updated = await value.Message.Updated.GetPackageAsync(_packageProvider, CancellationToken.None))
                using (var original = await value.Message.Original.GetPackageAsync(_packageProvider, CancellationToken.None))
                {
                    foreach (var rule in _rules)
                    {
                        _writer.WriteLine(rule.Name);

                        var results = await rule.RunRuleAsync(original, updated, CancellationToken.None);

                        finalResults.AddRange(results);
                    }
                }

                await _storage.UpdateAsync(new IssueResults
                {
                    Id = value.Message.Id,
                    Issues = finalResults,
                    State = IssueResultState.Completed
                }, CancellationToken.None);

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
