using AutoMapper;

namespace CompatibilityAnalyzer.Models.Protobuf
{
    internal class DeserializingProfile : Profile
    {
        public DeserializingProfile()
        {
            CreateMap<AnalyzeRequest, Models.AnalyzeRequest>();
            CreateMap<NuGetRequest, NuGetRequestItem>();
            CreateMap<RequestItem, IRequestItem>()
                .ConvertUsing((request, result, ctx) =>
                {
                    switch (request.ItemCase)
                    {
                        case RequestItem.ItemOneofCase.Nuget:
                            return ctx.Mapper.Map<NuGetRequestItem>(request.Nuget);
                        default:
                            return null;
                    }
                });
        }
    }
}
