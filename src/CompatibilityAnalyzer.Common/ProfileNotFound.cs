using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    public class ProfileNotFound : CompatibilityAnalysisException
    {
        public ProfileNotFound(string profile)
            : base($"Could not find profile '{profile}'")
        {
            Profile = profile;
        }

        public string Profile { get; }
    }
}
