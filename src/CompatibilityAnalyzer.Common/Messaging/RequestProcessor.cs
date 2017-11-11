using CompatibilityAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Messaging
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly IEnumerable<IAnalyzerRule> _rules;
        private readonly INuGetPackageProvider _packageProvider;
        private readonly IResultStorage _storage;
        private readonly TextWriter _writer;

        public RequestProcessor(IEnumerable<IAnalyzerRule> rules, INuGetPackageProvider packageProvider, IResultStorage storage, TextWriter writer)
        {
            _rules = rules;
            _packageProvider = packageProvider;
            _storage = storage;
            _writer = writer;
        }

        public async Task ProcessAsync(AnalyzeRequest request, CancellationToken token)
        {
            try
            {
                var finalResults = new List<Issue>();

                Console.WriteLine($"Starting: {request.Id}");

                await _storage.UpdateAsync(new IssueResults
                {
                    Id = request.Id,
                    Issues = Array.Empty<Issue>(),
                    State = IssueResultState.Processing
                }, CancellationToken.None);

                using (var updated = await request.Updated.GetPackageAsync(_packageProvider, CancellationToken.None))
                using (var original = await request.Original.GetPackageAsync(_packageProvider, CancellationToken.None))
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
                    Id = request.Id,
                    Issues = finalResults,
                    State = IssueResultState.Completed
                }, CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
