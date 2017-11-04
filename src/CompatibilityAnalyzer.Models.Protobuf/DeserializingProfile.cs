using AutoMapper;

namespace CompatibilityAnalyzer.Models.Protobuf
{
    internal class DeserializingProfile : Profile
    {
        public DeserializingProfile()
        {
            CreateMap<AnalyzeRequest, Models.AnalyzeRequest>();
            CreateMap<NuGetRequest, NugetData>();
            CreateMap<RequestItem, IRequestItem>()
                .ConvertUsing((request, result, ctx) =>
                {
                    switch (request.ItemCase)
                    {
                        case RequestItem.ItemOneofCase.Nuget:
                            return ctx.Mapper.Map<NugetData>(request.Nuget);
                        default:
                            return null;
                    }
                });
        }
    }
}
