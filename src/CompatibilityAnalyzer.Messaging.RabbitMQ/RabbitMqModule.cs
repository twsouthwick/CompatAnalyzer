using Autofac;
using RabbitMQ.Client;

namespace CompatibilityAnalyzer.Messaging
{
    public class RabbitMqModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(_ =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = "rabbitmq",
                    VirtualHost = "/"
                };

                return factory.CreateConnection();
            })
            .As<IConnection>()
            .SingleInstance();

            builder.RegisterAdapter<IConnection, IModel>(conn =>
            {
                var model = conn.CreateModel();

                model.ExchangeDeclare(Constants.Exchange, ExchangeType.Direct);
                model.QueueDeclare(Constants.MessageQueue, false, false, false, null);
                model.QueueBind(Constants.MessageQueue, Constants.Exchange, Constants.Routing, null);

                return model;
            }).SingleInstance();

            builder.RegisterType<RabbitMqConsumer>()
                .As<IRequestObservable>()
                .SingleInstance();

            builder.RegisterType<RabbitMqQueue>()
                .As<IRequestQueue>()
                .SingleInstance();
        }
    }
}
