using System;
using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public interface IPackage : IDisposable
    {
        string Id { get; }

        string Version { get; }

        IReadOnlyCollection<FrameworkInfo> SupportedFrameworks { get; }

        FrameworkItems GetAssemblies(FrameworkInfo framework);
    }
}
