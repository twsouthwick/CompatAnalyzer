using Autofac;
using NuGet.Common;
using NuGet.Protocol.Core.Types;

namespace CompatibilityAnalyzer
{
    internal class NuGetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SourceCacheContext>()
                .SingleInstance();

            builder.RegisterType<NuGetPackageDownloader>()
                .As<INuGetPackageProvider>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TextWriterLogger>()
                .As<ILogger>()
                .InstancePerLifetimeScope();

            builder.RegisterType<NuGetDownloaderSettings>()
                .SingleInstance();
        }
    }
}
