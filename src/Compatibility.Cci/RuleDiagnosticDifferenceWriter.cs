using Microsoft.Cci;
using Microsoft.Cci.Traversers;
using Microsoft.Cci.Writers;
using System;
using System.Collections.Generic;
using Microsoft.Cci.Filters;
using Microsoft.Cci.Mappings;

namespace CompatibilityAnalyzer
{
    public interface IDifferenceCollecctor : IObservable<RuleDiagnostic>, ICciDifferenceWriter, IDisposable
    {

    }

    public class RuleDiagnosticDifferenceWriter : DifferenceTraverser, IDifferenceCollecctor
    {
        private readonly List<IObserver<RuleDiagnostic>> _diagnostics = new List<IObserver<RuleDiagnostic>>();

        public RuleDiagnosticDifferenceWriter(MappingSettings settings, IDifferenceFilter filter)
            : base(settings, filter)
        {
        }

        public void Dispose()
        {
            _diagnostics.Clear();
        }

        public IDisposable Subscribe(IObserver<RuleDiagnostic> observer)
        {
            _diagnostics.Add(observer);

            return new DelegateDisposable(() => _diagnostics.Remove(observer));
        }

        private class DelegateDisposable : IDisposable
        {
            private readonly Action _action;

            public DelegateDisposable(Action action) => _action = action;

            public void Dispose() => _action();
        }

        public void Write(string oldAssembliesName, IEnumerable<IAssembly> oldAssemblies, string newAssembliesName, IEnumerable<IAssembly> newAssemblies)
        {
        }
    }
}
