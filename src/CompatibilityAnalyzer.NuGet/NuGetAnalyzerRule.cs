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
        private readonly INuGetPackageProvider _packageProvider;

        public NuGetAnalyzerRule(IAssemblyCompatibilityAnalyzer analyzer, INuGetPackageProvider packageProvider)
        {
            _analyzer = analyzer;
            _packageProvider = packageProvider;
        }

        public string Name => "NuGet target analyzer";

        public async Task RunRuleAsync(CancellationToken token)
        {
            using (var result = await _packageProvider.GetPackageAsync("Newtonsoft.Json", "10.0.2", CancellationToken.None))
            using (var result2 = await _packageProvider.GetPackageAsync("Newtonsoft.Json", "10.0.1", CancellationToken.None))
            {
                var files = new List<IFile>();

                var list1a = result.Frameworks
                    .SelectMany(NuGet.Frameworks.CompatibilityListProvider.Default.GetFrameworksSupporting)
                    .ToHashSet();

                var list1b = list1a.ToHashSet();

                var list2 = result2.Frameworks
                    .SelectMany(NuGet.Frameworks.CompatibilityListProvider.Default.GetFrameworksSupporting)
                    .ToHashSet();

                list1a.RemoveWhere(list2.Contains);
                list2.RemoveWhere(list1b.Contains);

                foreach (var framework in result.Frameworks)
                {
                    Console.WriteLine(framework);

                    foreach (var assembly in result.GetAssemblies(framework))
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
}