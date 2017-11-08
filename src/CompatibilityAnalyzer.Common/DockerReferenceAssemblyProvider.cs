using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompatibilityAnalyzer
{
    public class DockerReferenceAssemblyProvider : IReferenceAssemblyProvider
    {
        private const string Folder = "/refs";

        public IEnumerable<string> SupportedProfiles => Directory.GetDirectories(Folder).Select(Path.GetDirectoryName);

        public IEnumerable<string> GetReferenceAssemblyPath(string profile)
        {
            var folder = Path.Combine(Folder, profile);

            if (!Directory.Exists(folder))
            {
                throw new ProfileNotFound(profile);
            }

            yield return folder;
        }
    }
}
