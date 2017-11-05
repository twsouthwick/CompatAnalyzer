using CompatibilityAnalyzer.Messaging;
using CompatibilityAnalyzer.Models;
using CompatibilityAnalyzer.Service.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Service.Controllers
{
    [Route("api/[controller]")]
    public class AnalyzerController : Controller
    {
        private readonly IRequestQueue _queue;

        public AnalyzerController(IRequestQueue queue)
        {
            _queue = queue;
        }

        [HttpPost("NuGet")]
        public async Task<ResultId> Nuget([FromBody]NuGetRequest nuGetRequest)
        {
            var request = new AnalyzeRequest
            {
                Id = Guid.NewGuid(),
                Original = nuGetRequest.Original,
                Updated = nuGetRequest.Updated
            };

            await _queue.AddAsync(request);

            return new ResultId
            {
                Id = request.Id
            };
        }

        public class NuGetRequest
        {
            public NugetData Original { get; set; }

            public NugetData Updated { get; set; }
        }
    }
}
