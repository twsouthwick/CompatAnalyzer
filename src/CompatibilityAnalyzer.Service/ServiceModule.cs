using Autofac;
using CompatibilityAnalyzer.Messaging.RabbitMQ;
using CompatibilityAnalyzer.Models;
using CompatibilityAnalyzer.Models.Protobuf;

namespace CompatibilityAnalyzer.Service
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProtobufModelSerializer>()
                .As<IModelSerializer>()
                .SingleInstance();

            builder.RegisterModule<RabbitMqModule>();
        }
    }
}
