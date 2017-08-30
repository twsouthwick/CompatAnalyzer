using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NuGetCompatAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var analyzer = new ApiCompat.Analyzer(Console.Out);

            using (var downloader = new NuGetPackageDownloader(@"https://api.nuget.org/v3/index.json"))
            {
                var result = await downloader.DownloadAsync("Newtonsoft.Json", "10.0.3", CancellationToken.None);

                foreach (var framework in result.GetFrameworks())
                {
                    Console.WriteLine(framework);

                    foreach (var assembly in result.GetAssemblies(framework))
                    {
                        Console.WriteLine($"\t{assembly.Path}");
                    }
                }
            }

            var _604 = @"C:\Users\tasou\.nuget\packages\newtonsoft.json\6.0.4\lib\net45\Newtonsoft.Json.dll";
            var _10 = @"C:\Users\tasou\.nuget\packages\newtonsoft.json\10.0.3\lib\net45\Newtonsoft.Json.dll";
            //analyzer.Analyze(new[] { new FileAssemblyFile(_604) }, new[] { new FileAssemblyFile(_10) });
        }
    }
}
