using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CompatibilityAnalyzer
{
    [Command(AnalysisCommand.CollectReferenceAssemblies)]
    internal class ReferenceAssemblyCommand : ICommand
    {
        private readonly TextWriter _writer;
        private readonly IReferenceAssemblyProvider _provider;
        private readonly IReferenceAssemblyOptions _options;

        public ReferenceAssemblyCommand(TextWriter writer, IReferenceAssemblyProvider provider, IReferenceAssemblyOptions options)
        {
            _writer = writer;
            _provider = provider;
            _options = options;
        }

        public Task RunAsync()
        {
            using (var fs = new FileStream(_options.ReferencePath, FileMode.Create, FileAccess.Write))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                foreach (var profile in _provider.SupportedProfiles)
                {
                    _writer.WriteLine($"Adding reference assembly for: {profile}");

                    foreach (var item in Directory.EnumerateFiles(_provider.GetReferenceAssemblyPath(profile), "*.dll", SearchOption.TopDirectoryOnly))
                    {
                        var name = Path.Combine(profile, Path.GetFileName(item));

                        zip.CreateEntryFromFile(item, name, CompressionLevel.Optimal);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
