using AutoMapper;
using Google.Protobuf;
using System;
using System.IO;

namespace CompatibilityAnalyzer.Models.Protobuf
{
    public class ProtobufModelSerializer : IModelSerializer
    {
        private readonly Lazy<IMapper> _mapper;

        public ProtobufModelSerializer()
        {
            _mapper = new Lazy<IMapper>(CreateMapper);
        }

        public Models.AnalyzeRequest Deserialize(Stream stream)
        {
            var request = AnalyzeRequest.Parser.ParseFrom(stream);

            return _mapper.Value.Map<Models.AnalyzeRequest>(request);
        }

        public void Serialize(Models.AnalyzeRequest request, Stream stream)
        {
            var v = _mapper.Value.Map<AnalyzeRequest>(request);

            v.WriteTo(stream);
        }

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SerializingProfile>();
                cfg.AddProfile<DeserializingProfile>();
            });

            config.AssertConfigurationIsValid();

            return config.CreateMapper();
        }
    }
}
