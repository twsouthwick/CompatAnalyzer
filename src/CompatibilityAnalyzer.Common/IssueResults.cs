using System;
using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public class IssueResults
    {
        public Guid Id { get; set; }

        public IReadOnlyCollection<Issue> Issues { get; set; }

        public IssueResultState State { get; set; }
    }
}
