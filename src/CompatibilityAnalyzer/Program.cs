using Autofac;
using System;
using System.IO;
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
            var result = await _downloader.DownloadAsync("Newtonsoft.Json", "10.0.2", CancellationToken.None);

            foreach (var framework in result.GetFrameworks())
            {
                Console.WriteLine(framework);

                foreach (var assembly in result.GetAssemblies(framework))
                {
                    Console.WriteLine($"\t{assembly.Path}");
                }
            }

            var _604 = @"C:\Users\tasou\.nuget\packages\newtonsoft.json\6.0.4\lib\net45\Newtonsoft.Json.dll";
            var _10 = @"C:\Users\tasou\.nuget\packages\newtonsoft.json\10.0.3\lib\net45\Newtonsoft.Json.dll";
            //analyzer.Analyze(new[] { new FileAssemblyFile(_604) }, new[] { new FileAssemblyFile(_10) });
        }
    }
}
