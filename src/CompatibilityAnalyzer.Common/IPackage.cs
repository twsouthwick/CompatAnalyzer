using System.Collections.Generic;
using System;

namespace CompatibilityAnalyzer
{
    public interface IPackage : IDisposable
    {
        string Id { get; }

        string Version { get; }

        IReadOnlyCollection<INuGetFramework> Frameworks { get; }

        IEnumerable<IFile> GetAssemblies(INuGetFramework framework);
    }
}