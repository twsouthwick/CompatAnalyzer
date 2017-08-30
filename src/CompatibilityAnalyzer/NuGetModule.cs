using Autofac;
using NuGet.Common;
using NuGet.Protocol.Core.Types;

namespace CompatibilityAnalyzer
{
    class NuGetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SourceCacheContext>()
                    .SingleInstance();

            builder.RegisterType<NuGetPackageDownloader>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<TextWriterLogger>()
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterInstance(GetSettings());
        }

        private static NuGetDownloaderSettings GetSettings()
        {
            return new NuGetDownloaderSettings
            {
                Feed = @"https://api.nuget.org/v3/index.json"
            };
        }
    }
}
