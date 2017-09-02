using System;
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

            // builder.RegisterType<NuGetPackageDownloader>()
            builder.RegisterType<LocalNuGetPackageProvider>()
                .As<INuGetPackageProvider>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TextWriterLogger>()
                .As<ILogger>()
                .InstancePerLifetimeScope();

            builder.RegisterInstance(GetSettings());
        }

        private static NuGetDownloaderSettings GetSettings()
        {
            return new NuGetDownloaderSettings
            {
                // Feed = @"https://api.nuget.org/v3/index.json"
                Feed = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
        }
    }
}
