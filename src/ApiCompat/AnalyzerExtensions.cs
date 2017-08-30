using Microsoft.Cci;
using Microsoft.Cci.Extensions;
using NuGetCompatAnalyzer;
using System.Collections.Generic;

namespace ApiCompat
{
    internal static class AnalyzerExtensions
    {
        public static IEnumerable<IAssembly> LoadAssemblies(this HostEnvironment host, IEnumerable<IAssemblyFile> files)
        {
            var result = new List<IAssembly>();

            foreach (var file in files)
            {
                using (var stream = file.OpenReadAsync())
                {
                    result.Add(host.LoadAssemblyFrom(file.Path, stream));
                }
            }

            return result;
        }
    }
}
