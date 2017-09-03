using System.Collections.Generic;
using System;

namespace CompatibilityAnalyzer
{
    public interface IPackage : IDisposable
    {
        string Id { get; }

        string Version { get; }

        IReadOnlyCollection<FrameworkInfo> SupportedFrameworks { get; }

        IEnumerable<IFile> GetAssemblies(FrameworkInfo framework);
    }
}