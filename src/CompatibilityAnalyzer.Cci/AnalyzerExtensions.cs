using Microsoft.Cci;
using Microsoft.Cci.Extensions;
using System.Collections.Generic;

namespace CompatibilityAnalyzer
{
    internal static class AnalyzerExtensions
    {
        public static IEnumerable<IAssembly> LoadAssemblies(this HostEnvironment host, IEnumerable<IFile> files)
        {
            var result = new List<IAssembly>();

            foreach (var file in files)
            {
                using (var stream = file.OpenRead())
                {
                    result.Add(host.LoadAssemblyFrom(file.Path, stream));
                }
            }

            return result;
        }
    }
}
