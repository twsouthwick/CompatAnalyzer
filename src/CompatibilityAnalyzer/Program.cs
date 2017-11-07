using Autofac;
using CompatibilityAnalyzer.Messaging;
using CompatibilityAnalyzer.Models;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var version = typeof(Program).Assembly.GetName().Version;

            Console.WriteLine($"Package Compatibility Analyzer v{version}{Environment.NewLine}");

            await Task.Delay(2000);

            try
            {
                var options = AnalysisOptionsBuilder.Parse(args);

                using (var container = CreateContainer(options))
                {
                    await container.Resolve<ICommand>().RunAsync();
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

        private static IContainer CreateContainer(AnalysisOptionsBuilder options)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance<TextWriter>(Console.Out);
            builder.RegisterType<Program>()
                .AsSelf();

            builder.RegisterInstance(options)
                .As<IReferenceAssemblyOptions>()
                .As<IAnalysisOptions>();

            builder.RegisterModule<NuGetModule>();
            builder.RegisterModule<AssemblyCompatibilityModule>();
            builder.RegisterModule<RulesModule>();
            builder.RegisterModule<RabbitMqModule>();
            builder.RegisterModule<ProtobufModule>();

            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => !t.IsAbstract && t.IsAssignableTo<ICommand>())
                .Where(t => t.GetCustomAttribute<CommandAttribute>()?.Command == options.Command)
                .As<ICommand>();

            return builder.Build();
        }
    }
}
