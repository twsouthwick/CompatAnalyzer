using CompatibilityAnalyzer.Models;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Messaging
{
    internal class RabbitMqQueue : IRequestQueue
    {
        private readonly IModel _model;
        private readonly IModelSerializer _serializer;

        public RabbitMqQueue(IModel model, IModelSerializer serializer)
        {
            _model = model;
            _serializer = serializer;
        }

        public Task AddAsync(AnalyzeRequest request)
        {
            var bytes = _serializer.Serialize(request);

            _model.BasicPublish(Constants.Exchange, Constants.Routing, null, bytes);

            return Task.CompletedTask;
        }
    }
}
