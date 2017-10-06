using Autofac;

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

            builder.Register<IReferenceAssemblyProvider>(ctx =>
            {
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
