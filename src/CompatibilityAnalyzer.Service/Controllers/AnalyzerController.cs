using CompatibilityAnalyzer.Messaging;
using CompatibilityAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer.Service.Controllers
{
    [Route("api/[controller]")]
    public class AnalyzerController : Controller
    {
        private readonly IRequestQueue _queue;
        private readonly IResultStorage _storage;

        public AnalyzerController(IRequestQueue queue, IResultStorage storage)
        {
            _queue = queue;
            _storage = storage;
        }

        [HttpGet("{id}")]
        public Task<IssueResults> GetAsync(Guid id)
        {
            return _storage.GetAsync(id, CancellationToken.None);
        }

        [HttpPost("NuGet")]
        public async Task<IssueResults> Nuget([FromBody]NuGetRequest nuGetRequest)
        {
            var results = await _storage.CreateAsync(CancellationToken.None);

            var request = new AnalyzeRequest
            {
                Id = results.Id,
                Original = nuGetRequest.Original,
                Updated = nuGetRequest.Updated
            };

            await _queue.AddAsync(request);

            return results;
        }

        public class NuGetRequest
        {
            public NuGetRequestItem Original { get; set; }

            public NuGetRequestItem Updated { get; set; }
        }
    }
}
