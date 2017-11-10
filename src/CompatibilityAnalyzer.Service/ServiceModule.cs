using Autofac;
using CompatibilityAnalyzer.Messaging;
using CompatibilityAnalyzer.Models;

namespace CompatibilityAnalyzer.Service
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<RabbitMqModule>();
            builder.RegisterModule<ProtobufModule>();
            builder.RegisterModule<MongoDbModule>();
        }
    }
}
