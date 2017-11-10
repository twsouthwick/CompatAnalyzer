using System;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public interface IResultStorage
    {
        Task<IssueResults> CreateAsync(CancellationToken token);

        Task UpdateAsync(IssueResults results, CancellationToken token);

        Task<IssueResults> GetAsync(Guid guid, CancellationToken token);
    }
}