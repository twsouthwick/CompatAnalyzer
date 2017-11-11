using CompatibilityAnalyzer.Models;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Messaging
{
    internal class RabbitMqConsumer : IQueueListener
    {
        private readonly IModel _model;
        private readonly IModelSerializer _serializer;
        private readonly IRequestProcessor _requestProcessor;

        public RabbitMqConsumer(IModel model, IModelSerializer serializer, IRequestProcessor requestProcessor)
        {
            _model = model;
            _serializer = serializer;
            _requestProcessor = requestProcessor;
        }

        public async Task ProcessQueueAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var result = _model.BasicGet(Constants.MessageQueue, false);

                if (result != null)
                {
                    var request = _serializer.Deserialize(result.Body);

                    try
                    {
                        await _requestProcessor.ProcessAsync(request, token);
                        _model.BasicAck(result.DeliveryTag, false);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
