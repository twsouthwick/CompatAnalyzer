using Autofac;

namespace CompatibilityAnalyzer
{
    internal class RulesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(NuGetAnalyzerRule).Assembly)
                .AssignableTo(typeof(IAnalyzerRule))
                .As<IAnalyzerRule>()
                .SingleInstance();
        }
    }
}
