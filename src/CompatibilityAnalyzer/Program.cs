using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    internal class Program
    {
        private readonly IEnumerable<IAnalyzerRule> _rules;
        private readonly INuGetPackageProvider _packageProvider;
        private readonly IAnalysisOptions _options;
        private readonly TextWriter _writer;

        public Program(IEnumerable<IAnalyzerRule> rules, TextWriter writer, INuGetPackageProvider packageProvider, IAnalysisOptions options)
        {
            _rules = rules;
            _writer = writer;
            _packageProvider = packageProvider;
            _options = options;
        }

        private static async Task Main(string[] args)
        {
            var version = typeof(Program).Assembly.GetName().Version;

            Console.WriteLine($"Package Compatibility Analyzer v{version}{Environment.NewLine}");

            try
            {
                var options = AnalysisOptionsBuilder.Parse(args);

                using (var container = CreateContainer(options))
                {
                    var program = container.Resolve<Program>();

                    await program.RunAsync();
                }
            }
            catch (CompatibilityAnalysisException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static IContainer CreateContainer(IAnalysisOptions options)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance<TextWriter>(Console.Out);
            builder.RegisterType<Program>()
                .AsSelf();

            builder.RegisterInstance(options);

            builder.RegisterModule<NuGetModule>();
            builder.RegisterModule<AssemblyCompatibilityModule>();
            builder.RegisterModule<RulesModule>();

            return builder.Build();
        }

        public async Task RunAsync()
        {
            using (var updated = await _packageProvider.GetPackageAsync(_options.PackageName, _options.UpdatedVersion, CancellationToken.None))
            using (var original = await _packageProvider.GetPackageAsync(_options.PackageName, _options.OriginalVersion, CancellationToken.None))
            {
                foreach (var rule in _rules)
                {
                    _writer.WriteLine(rule.Name);

                    await rule.RunRuleAsync(original, updated, CancellationToken.None);
                }
            }
        }
    }
}
