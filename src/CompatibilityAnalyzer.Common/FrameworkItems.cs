using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    public struct FrameworkItems
    {
        public FrameworkItems(IEnumerable<IFile> files, IEnumerable<IFile> dependencies)
        {
            Files = files;
            Dependencies = dependencies;
        }

        public IEnumerable<IFile> Files { get; }

        public IEnumerable<IFile> Dependencies { get; }
    }
}
