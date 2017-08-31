using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    internal class Program
    {
        private readonly IAssemblyCompatibilityAnalyzer _analyzer;
        private readonly NuGetPackageDownloader _downloader;

        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance<TextWriter>(Console.Out);
            builder.RegisterType<Program>()
                .AsSelf();

            builder.RegisterModule<NuGetModule>();
            builder.RegisterModule<AssemblyCompatibilityModule>();

            using (var container = builder.Build())
            {
                var program = container.Resolve<Program>();

                program.RunAsync().GetAwaiter().GetResult();
            }
        }

        public Program(IAssemblyCompatibilityAnalyzer analyzer, NuGetPackageDownloader downloader)
        {
            _analyzer = analyzer;
            _downloader = downloader;
        }

        public async Task RunAsync()
        {
            using (var result = await _downloader.DownloadAsync("Newtonsoft.Json", "10.0.2", CancellationToken.None))
            using (var result2 = await _downloader.DownloadAsync("Newtonsoft.Json", "6.0.8", CancellationToken.None))
            {
                var files = new List<IFile>();

                var list1 = result.Frameworks
                    .SelectMany(NuGet.Frameworks.CompatibilityListProvider.Default.GetFrameworksSupporting)
                    .ToHashSet();

                var list2 = result2.Frameworks
                    .SelectMany(NuGet.Frameworks.CompatibilityListProvider.Default.GetFrameworksSupporting)
                    .ToHashSet();

                list1.RemoveWhere(list2.Contains);

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
