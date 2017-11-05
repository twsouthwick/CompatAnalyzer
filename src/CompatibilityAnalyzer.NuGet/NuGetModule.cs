using Autofac;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System;

namespace CompatibilityAnalyzer
{
    public class NuGetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(_ => new SourceCacheContext
            {
                DirectDownload = false,
                IgnoreFailedSources = true,
                MaxAge = DateTimeOffset.MinValue,
                NoCache = true
            })
            .As<SourceCacheContext>()
            .SingleInstance();

            builder.RegisterType<NuGetPackageDownloader>()
                .As<INuGetPackageProvider>()
                .InstancePerLifetimeScope();

            builder.RegisterAdapter<NuGetDownloaderSettings, SourceRepository>(settings => Repository.Factory.GetCoreV3(settings.Feed))
                .SingleInstance();

            builder.RegisterType<TextWriterLogger>()
                .As<ILogger>()
                .InstancePerLifetimeScope();

            builder.RegisterType<NuGetDownloaderSettings>()
                .SingleInstance();
        }
    }
}
