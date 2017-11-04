using CompatibilityAnalyzer.Models;
using CompatibilityAnalyzer.Service.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CompatibilityAnalyzer.Service.Controllers
{
    [Route("api/[controller]")]
    public class AnalyzerController : Controller
    {
        private readonly IModelSerializer _serializer;

        public AnalyzerController(IModelSerializer serializer)
        {
            _serializer = serializer;
        }

        [HttpPost("NuGet")]
        public ResultId Nuget([FromBody]NuGetRequest nuGetRequest)
        {
            var request = new AnalyzeRequest
            {
                Id = Guid.NewGuid(),
                Original = nuGetRequest.Original,
                Updated = nuGetRequest.Updated
            };

            var bytes = _serializer.Serialize(request);
            var deserialized = _serializer.Deserialize(bytes);

            return new ResultId
            {
                Id = Guid.NewGuid()
            };
        }
    }

    public class NuGetRequest
    {
        public NugetData Original { get; set; }

        public NugetData Updated { get; set; }
    }
}
