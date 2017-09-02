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
        private readonly IEnumerable<IAnalyzerRule> _rules;
        private readonly INuGetPackageProvider _packageProvider;
        private readonly TextWriter _writer;

        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance<TextWriter>(Console.Out);
            builder.RegisterType<Program>()
                .AsSelf();

            builder.RegisterModule<NuGetModule>();
            builder.RegisterModule<AssemblyCompatibilityModule>();
            builder.RegisterModule<RulesModule>();

            using (var container = builder.Build())
            {
                var program = container.Resolve<Program>();

                program.RunAsync().GetAwaiter().GetResult();
            }
        }

        public Program(IEnumerable<IAnalyzerRule> rules, TextWriter writer, INuGetPackageProvider packageProvider)
        {
            _rules = rules;
            _writer = writer;
            _packageProvider = packageProvider;
        }

        public async Task RunAsync()
        {
            using (var updated = await _packageProvider.GetPackageAsync("Newtonsoft.Json", "10.0.2", CancellationToken.None))
            using (var original = await _packageProvider.GetPackageAsync("Newtonsoft.Json", "10.0.1", CancellationToken.None))
            {
                foreach (var rule in _rules)
                {
                    _writer.Write(rule.Name);

                    await rule.RunRuleAsync(original, updated, CancellationToken.None);
                }
            }
        }
    }
}
