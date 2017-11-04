using CompatibilityAnalyzer.Service.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CompatibilityAnalyzer.Service.Controllers
{
    [Route("api/[controller]")]
    public class AnalyzerController : Controller
    {
        [HttpPost("NuGet")]
        public ResultId Nuget([FromBody]NugetData value)
        {
            return new ResultId
            {
                Id = Guid.NewGuid()
            };
        }
    }
}
