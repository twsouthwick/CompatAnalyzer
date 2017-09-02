using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public class NuGetAnalyzerRule : IAnalyzerRule
    {
        private readonly IAssemblyCompatibilityAnalyzer _analyzer;

        public NuGetAnalyzerRule(IAssemblyCompatibilityAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        public string Name => "NuGet target analyzer";

        public async Task RunRuleAsync(IPackage original, IPackage updated, CancellationToken token)
        {
            var files = new List<IFile>();

            var list1a = original.Frameworks
                .Select(f => f.AsFramework())
                .SelectMany(NuGet.Frameworks.CompatibilityListProvider.Default.GetFrameworksSupporting)
                .ToHashSet();

            var list1b = list1a.ToHashSet();

            var list2 = updated.Frameworks
                .Select(f => f.AsFramework())
                .SelectMany(NuGet.Frameworks.CompatibilityListProvider.Default.GetFrameworksSupporting)
                .ToHashSet();

            list1a.RemoveWhere(list2.Contains);
            list2.RemoveWhere(list1b.Contains);

            foreach (var framework in original.Frameworks)
            {
                Console.WriteLine(framework);

                foreach (var assembly in original.GetAssemblies(framework))
                {
                    Console.WriteLine($"\t{assembly.Path}");

                    files.Add(assembly);
                }
            }

            Console.WriteLine($"Comparing {files[0]} and {files[1]}");

            _analyzer.Analyze(new[] { files[0] }, new[] { files[1] });
        }
    }
}