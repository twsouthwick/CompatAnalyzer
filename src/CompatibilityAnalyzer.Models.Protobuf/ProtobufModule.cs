using Autofac;
using CompatibilityAnalyzer.Models.Protobuf;

namespace CompatibilityAnalyzer.Models
{
    public class ProtobufModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProtobufModelSerializer>()
                .As<IModelSerializer>()
                .SingleInstance();
        }
    }
}
