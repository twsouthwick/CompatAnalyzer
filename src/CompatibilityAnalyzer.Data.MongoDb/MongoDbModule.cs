using Autofac;

namespace CompatibilityAnalyzer
{
    public class MongoDbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MongoDbStorage>()
                .As<IResultStorage>()
                .SingleInstance();
        }
    }
}
