using NuGet.Frameworks;
using System;
using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public interface IPackage : IDisposable
    {
        string Id { get; }

        string Version { get; }

        IReadOnlyCollection<NuGetFramework> SupportedFrameworks { get; }

        FrameworkItems GetAssemblies(NuGetFramework framework);
    }
}
