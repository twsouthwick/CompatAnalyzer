using System.Collections.Generic;
using NuGet.Frameworks;
using System;

namespace CompatibilityAnalyzer
{
    public interface IPackage : IDisposable
    {
        string Id { get; }

        string Version { get; }

        IReadOnlyCollection<NuGetFramework> Frameworks { get; }

        IEnumerable<IFile> GetAssemblies(NuGetFramework framework);
    }
}