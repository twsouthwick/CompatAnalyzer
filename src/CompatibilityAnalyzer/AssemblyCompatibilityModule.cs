using Autofac;
using System.Runtime.InteropServices;

namespace CompatibilityAnalyzer
{
    internal class AssemblyCompatibilityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CciAssemblyCompatibilityAnalyzer>()
                .As<IAssemblyCompatibilityAnalyzer>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProgramFilesReferenceAssembly>()
                .AsSelf();

            builder.RegisterType<ZipFileReferenceAssemblyProvider>()
                .AsSelf();

            builder.RegisterType<DockerReferenceAssemblyProvider>()
                .AsSelf();

            builder.Register<IReferenceAssemblyProvider>(ctx =>
            {
                // TODO: Have this depend on something more accurate instead of just OS
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return ctx.Resolve<DockerReferenceAssemblyProvider>();
                }

                var options = ctx.Resolve<IReferenceAssemblyOptions>();

                if (options.Create || string.IsNullOrEmpty(options.ReferencePath))
                {
                    return ctx.Resolve<ProgramFilesReferenceAssembly>();
                }
                else
                {
                    return ctx.Resolve<ZipFileReferenceAssemblyProvider>();
                }
            })
            .As<IReferenceAssemblyProvider>()
            .SingleInstance();
        }
    }
}
