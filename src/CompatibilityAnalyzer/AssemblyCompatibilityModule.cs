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
        }
    }
}
