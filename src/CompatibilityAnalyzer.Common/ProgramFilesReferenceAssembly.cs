using System;
using System.Collections.Generic;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class ProgramFilesReferenceAssembly : IReferenceAssemblyProvider
    {
        private static readonly string ReferenceDirectory = Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\");

        private readonly Lazy<IReadOnlyDictionary<string, string>> _map = new Lazy<IReadOnlyDictionary<string, string>>(() => CreateReferenceAssemblyLookup(), true);

        public IEnumerable<string> SupportedProfiles => _map.Value.Keys;

        public string GetReferenceAssemblyPath(string profile)
        {
            if (_map.Value.TryGetValue(profile, out var path))
            {
                return path;
            }
            else
            {
                throw new ProfileNotFound(profile);
            }
        }

        private static IReadOnlyDictionary<string, string> CreateReferenceAssemblyLookup()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "net35",  Path.Combine(ReferenceDirectory, ".NETFramework", "v3.5", "Profile", "Client") },
                { "net40",  Path.Combine(ReferenceDirectory, ".NETFramework", "v4.0") },
                { "net45",  Path.Combine(ReferenceDirectory, ".NETFramework", "v4.5") },
                { "net451", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.5.1") },
                { "net452", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.5.2") },
                { "net46",  Path.Combine(ReferenceDirectory, ".NETFramework", "v4.6.1") },
                { "net461", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.6.2") },
                { "net47",  Path.Combine(ReferenceDirectory, ".NETFramework", "v4.7") },
                { "netcore50",  Path.Combine(ReferenceDirectory, ".NETCore", "v4.5") }
            };

            var portables = new[]
            {
                Path.Combine(".NETPortable", "v4.0", "Profile"),
                Path.Combine(".NETPortable", "v4.5", "Profile"),
                Path.Combine(".NETPortable", "v4.6", "Profile"),
            };

            foreach (var portable in portables)
            {
                foreach (var directory in Directory.EnumerateDirectories(Path.Combine(ReferenceDirectory, portable)))
                {
                    var name = Path.GetFileName(directory);

                    map.Add(name, directory);
                }
            }

            return map;
        }
    }
}
