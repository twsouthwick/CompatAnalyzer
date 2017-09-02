using System;
using Autofac;

namespace CompatibilityAnalyzer
{
    internal class RulesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(INuGetPackageProvider).Assembly, typeof(CciAssemblyCompatibilityAnalyzer).Assembly)
                .AssignableTo(typeof(IAnalyzerRule))
                .As<IAnalyzerRule>()
                .SingleInstance();
        }
    }
}
