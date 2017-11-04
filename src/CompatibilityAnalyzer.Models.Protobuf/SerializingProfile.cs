using AutoMapper;
using System;

namespace CompatibilityAnalyzer.Models.Protobuf
{
    internal class SerializingProfile : Profile
    {
        public SerializingProfile()
        {
            CreateMap<string, string>()
                .ConvertUsing(s =>
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        throw new SerializationException("Strings must have values");
                    }

                    return s;
                });
            CreateMap<Guid, string>()
                .ConvertUsing(g => g.ToString());
            CreateMap<Models.AnalyzeRequest, AnalyzeRequest>();
            CreateMap<Models.NugetData, NuGetRequest>();
            CreateMap<IRequestItem, RequestItem>()
                .ConvertUsing((irequest, request, ctx) =>
                {
                    request = new RequestItem();

                    switch (irequest)
                    {
                        case NugetData nuget:
                            request.Nuget = ctx.Mapper.Map<NuGetRequest>(nuget);
                            break;
                        default:
                            throw new SerializationException($"Unknown request item type: {irequest.GetType()}");
                    }

                    return request;
                });
        }
    }
}
