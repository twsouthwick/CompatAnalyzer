using Autofac;

namespace CompatibilityAnalyzer
{
    public class RulesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(RulesModule).Assembly)
                .AssignableTo(typeof(IAnalyzerRule))
                .As<IAnalyzerRule>()
                .SingleInstance();
        }
    }
}
