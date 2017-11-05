using CompatibilityAnalyzer.Models;
using RabbitMQ.Client;
using System;
using System.Reactive.Subjects;

namespace CompatibilityAnalyzer.Messaging
{
    internal class RabbitMqConsumer : IDisposable, IRequestObservable
    {
        private readonly Subject<IMessage<AnalyzeRequest>> _subject;
        private readonly IModel _model;
        private readonly string _consumerTag;

        public RabbitMqConsumer(IModel model, IModelSerializer serializer)
        {
            _subject = new Subject<IMessage<AnalyzeRequest>>();
            _model = model;

            var consumer = new AnalyzeRequestConsumer(_model, serializer, _subject);
            _consumerTag = _model.BasicConsume(Constants.MessageQueue, false, consumer);
        }

        public void Dispose()
        {
            _model.BasicCancel(_consumerTag);
        }

        public IDisposable Subscribe(IObserver<IMessage<AnalyzeRequest>> observer) => _subject.Subscribe(observer);

        private sealed class AnalyzeRequestConsumer : DefaultBasicConsumer
        {
            private readonly IObserver<IMessage<AnalyzeRequest>> _observer;
            private readonly IModelSerializer _serializer;

            public AnalyzeRequestConsumer(IModel model, IModelSerializer serializer, IObserver<IMessage<AnalyzeRequest>> observer)
                : base(model)
            {
                _serializer = serializer;
                _observer = observer;
            }

            public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
            {
                // TODO: Error handling
                var request = _serializer.Deserialize(body);

                _observer.OnNext(new RabbitMqMessage<AnalyzeRequest>(request, Model, deliveryTag));
            }

            public override void HandleModelShutdown(object model, ShutdownEventArgs reason)
            {
                base.HandleModelShutdown(model, reason);
                _observer.OnCompleted();
            }
        }

        private sealed class RabbitMqMessage<T> : IMessage<T>
        {
            private readonly IModel _model;
            private readonly ulong _tag;

            public RabbitMqMessage(T message, IModel model, ulong tag)
            {
                Message = message;
                _model = model;
                _tag = tag;
            }

            public T Message { get; }

            public void Complete()
            {
                lock (_model)
                {
                    _model.BasicAck(_tag, false);
                }
            }
        }
    }
}
