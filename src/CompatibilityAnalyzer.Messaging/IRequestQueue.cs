using CompatibilityAnalyzer.Models;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Messaging
{
    public interface IRequestQueue
    {
        Task AddAsync(AnalyzeRequest request);
    }
}
