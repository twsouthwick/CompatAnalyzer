using System;
using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public class Issue
    {
        public Issue(string id, string message)
            : this(id, message, Array.Empty<Issue>())
        {
        }

        public Issue(string id, string message, IReadOnlyCollection<Issue> nested)
        {
            Id = id;
            Message = message;
            Nested = nested;
        }

        public string Id { get; }

        public string Message { get; }

        public IReadOnlyCollection<Issue> Nested { get; }
    }
}
