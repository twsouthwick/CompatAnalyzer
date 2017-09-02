using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReferenceAssemblyDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ReferenceDirectory = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\";

            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { Path.Combine(".NETFramework", "v3.5", "Profile", "Client"), "net35" },
                { Path.Combine(".NETFramework", "v4.0"), "net40" },
                { Path.Combine(".NETFramework", "v4.5"), "net45" },
                { Path.Combine(".NETFramework", "v4.5.1"), "net451" },
                { Path.Combine(".NETFramework", "v4.5.2"), "net452" },
                { Path.Combine(".NETFramework", "v4.6.1"), "net46" },
                { Path.Combine(".NETFramework", "v4.6.2"), "net461" },
                { Path.Combine(".NETFramework", "v4.7"), "net47" },
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

                    map.Add(directory.Replace(ReferenceDirectory, string.Empty), name);
                }
            }

            using (var fs = new FileStream("output.zip", FileMode.Create, FileAccess.Write))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                foreach (var path in map.OrderBy(t => t.Key))
                {
                    Console.WriteLine(path.Value);

                    var startPath = Path.Combine(ReferenceDirectory, path.Key);
                    foreach (var item in Directory.EnumerateFiles(startPath, "*.dll", SearchOption.TopDirectoryOnly))
                    {
                        var name = Path.Combine(path.Value, item.Replace(startPath + "\\", string.Empty));
                        zip.CreateEntryFromFile(item, name, CompressionLevel.Optimal);
                    }
                }
            }
        }
    }
}
