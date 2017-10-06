using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    internal interface ICommand
    {
        Task RunAsync();
    }
}
