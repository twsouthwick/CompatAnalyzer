using System;
using System.Collections.Generic;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class ProgramFilesReferenceAssembly : IReferenceAssemblyProvider
    {
        private static readonly string ReferenceDirectory = Environment.ExpandEnvironmentVariables(@"%ProgramFiles(x86)%\Reference Assemblies\Microsoft\Framework\");

        private readonly Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> _map = new Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<string>>>(() => CreateReferenceAssemblyLookup(), true);

        public IEnumerable<string> SupportedProfiles => _map.Value.Keys;

        public IEnumerable<string> GetReferenceAssemblyPath(string profile)
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

        private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> CreateReferenceAssemblyLookup()
        {
            var map = new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase);

            void Add(string name, string path)
            {
                var paths = new List<string> { path };
                var facades = new DirectoryInfo(Path.Combine(path, "Facades"));

                if (facades.Exists)
                {
                    paths.Add(facades.FullName);
                }

                map.Add(name, paths);
            }

            Add("net20", Path.Combine(ReferenceDirectory, ".NETFramework", "v3.5", "Profile", "Client"));
            Add("net35", Path.Combine(ReferenceDirectory, ".NETFramework", "v3.5", "Profile", "Client"));
            Add("net40", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.0"));
            Add("net45", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.5"));
            Add("net451", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.5.1"));
            Add("net452", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.5.2"));
            Add("net46", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.6.1"));
            Add("net461", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.6.2"));
            Add("net47", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.7"));
            Add("netcore50", Path.Combine(ReferenceDirectory, ".NETCore", "v4.5"));
            Add("monotouch", Path.Combine(ReferenceDirectory, "MonoTouch", "v1.0"));
            Add("netstandard1.0", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.5"));
            Add("netstandard1.1", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.5"));
            Add("netstandard1.2", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.5.1"));
            Add("netstandard1.3", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.6"));
            Add("netstandard1.4", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.6.1"));
            Add("netstandard1.5", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.6.1"));
            Add("netstandard1.6", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.6.1"));
            Add("netstandard2.0", Path.Combine(ReferenceDirectory, ".NETFramework", "v4.7"));

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

                    Add(name, directory);
                }
            }

            return map;
        }
    }
}
