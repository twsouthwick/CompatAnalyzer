using System;

namespace CompatibilityAnalyzer.Models
{
    public class AnalyzeRequest
    {
        public Guid Id { get; set; }

        public IRequestItem Original { get; set; }

        public IRequestItem Updated { get; set; }
    }
}
