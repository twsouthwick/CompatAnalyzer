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
        private readonly TextWriter _writer;

        public Program(IEnumerable<IAnalyzerRule> rules)
        {
            _rules = rules;
        }

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

        public Program(IEnumerable<IAnalyzerRule> rules, TextWriter writer)
        {
            _rules = rules;
            _writer = writer;
        }

        public async Task RunAsync()
        {
            foreach (var rule in _rules)
            {
                _writer.Write(rule.Name);

                await rule.RunRuleAsync(CancellationToken.None);
            }
        }
    }
}
