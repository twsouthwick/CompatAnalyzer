using NuGet.Frameworks;
using NuGet.Packaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompatibilityAnalyzer
{
    internal static class NugetExtensions
    {
        private static FrameworkReducer _reducer = new FrameworkReducer();

        public static IEnumerable<IFile> GetAssemblies(this PackageArchiveReader reader, NuGetFramework framework)
        {
            var bestMatch = _reducer.GetNearest(framework, reader.GetSupportedFrameworks());
            var libs = reader.GetLibItems()
                .FirstOrDefault(i => i.TargetFramework == bestMatch);

            if (libs == null)
            {
                yield break;
            }

            var items = new List<IFile>();

            foreach (var lib in libs.Items.Where(i => string.Equals(".dll", Path.GetExtension(i))))
            {
                byte[] GetBytes()
                {
                    using (var ms = new MemoryStream())
                    using (var entryStream = reader.GetStream(lib))
                    {
                        entryStream.CopyTo(ms);

                        return ms.ToArray();
                    }
                }

                yield return new VersionedFile(new DelegateFile(GetBytes, lib), reader.NuspecReader.GetVersion().ToFullString());
            }
        }
    }
}
