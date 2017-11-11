using Microsoft.Cci.Filters;
using Microsoft.Cci.Mappings;
using Microsoft.Cci.Traversers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace CompatibilityAnalyzer
{
    internal class DifferenceTracker : DifferenceTraverser
    {
        private readonly List<Microsoft.Cci.Differs.Difference> _differences;

        public DifferenceTracker(MappingSettings settings, IDifferenceFilter filter)
            : base(settings, filter)
        {
            _differences = new List<Microsoft.Cci.Differs.Difference>();
        }

        public override void Visit(AssemblyMapping mapping)
        {
            Contract.Assert(_differences.Count == 0);

            base.Visit(mapping);
        }

        public override void Visit(Microsoft.Cci.Differs.Difference difference)
        {
            _differences.Add(difference);
        }

        public IReadOnlyCollection<Issue> Differences
        {
            get
            {
                if (_differences.Count == 0)
                {
                    return Array.Empty<Issue>();
                }
                else
                {
                    return _differences
                        .Select(d => new Issue(d.Id, d.Message))
                        .ToList();
                }
            }
        }
    }
}
