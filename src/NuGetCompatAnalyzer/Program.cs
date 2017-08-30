using System;

namespace NuGetCompatAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var analyzer = new ApiCompat.Analyzer(Console.Out);

            foreach (var name in analyzer.GetRules())
            {
                Console.WriteLine(name);
            }

            var _604 = @"C:\Users\tasou\.nuget\packages\newtonsoft.json\6.0.4\lib\net45\Newtonsoft.Json.dll";
            var _10 = @"C:\Users\tasou\.nuget\packages\newtonsoft.json\10.0.3\lib\net45\Newtonsoft.Json.dll";
            analyzer.Analyze(new[] { _604 }, new[] { _10 });
        }
    }
}
