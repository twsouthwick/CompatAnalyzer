﻿using ApiCompat;
using Microsoft.Cci;
using Microsoft.Cci.Comparers;
using Microsoft.Cci.Differs;
using Microsoft.Cci.Extensions;
using Microsoft.Cci.Filters;
using Microsoft.Cci.Mappings;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CompatibilityAnalyzer
{
    public class CciAssemblyCompatibilityAnalyzer : IAssemblyCompatibilityAnalyzer
    {
        private readonly TextWriter _log;
        private readonly IReferenceAssemblyProvider _referenceAssemblyProvider;

        public CciAssemblyCompatibilityAnalyzer(TextWriter log, IReferenceAssemblyProvider referenceAssemblyProvider)
        {
            _log = log;
            _referenceAssemblyProvider = referenceAssemblyProvider;

            Trace.Listeners.Add(new TextWriterTraceListener(_log)
            {
                Filter = new EventTypeFilter(SourceLevels.Error | SourceLevels.Warning)
            });
        }

        private HostEnvironment CreateHostEnvironment(NameTable nameTable, NuGetFramework framework)
        {
            void UnableToResolve(object sender, UnresolvedReference<IUnit, AssemblyIdentity> e)
            {
                Trace.TraceError("Unable to resolve assembly '{0}' referenced by the implementation assembly '{1}'.", e.Unresolved, e.Referrer);
            }

            var host = new HostEnvironment(nameTable)
            {
                ResolveAgainstRunningFramework = true,
                UnifyToLibPath = s_unifyToLibPaths,
                LoadErrorTreatment = ErrorTreatment.TreatAsWarning
            };

            host.UnableToResolve += new EventHandler<UnresolvedReference<IUnit, AssemblyIdentity>>(UnableToResolve);

            foreach (var path in _referenceAssemblyProvider.GetReferenceAssemblyPath(framework.HasProfile ? framework.Profile : framework.GetShortFolderName()))
            {
                host.AddLibPath(path);
            }

            return host;
        }

        public IReadOnlyCollection<Issue> Analyze(FrameworkItems version1Assemblies, FrameworkItems version2Assemblies, NuGetFramework framework)
        {
            var filter = GetBaselineDifferenceFilter();
            var sharedNameTable = new NameTable();

            var contractHost = CreateHostEnvironment(sharedNameTable, framework);
            var contractAssemblies = contractHost.LoadAssemblies(version1Assemblies.Files);

            if (s_ignoreDesignTimeFacades)
                contractAssemblies = contractAssemblies.Where(a => !a.IsFacade());

            var implHost = CreateHostEnvironment(sharedNameTable, framework);
            var implAssemblies = implHost.LoadAssemblies(version2Assemblies.Files);

            var tracker = GetDifferenceWriter(_log, filter);
            tracker.Visit(implAssemblies, contractAssemblies);

            return tracker.Differences;
        }

        public IEnumerable<CompatibilityRule> GetRules()
        {
            var c = GetCompositionHost();
            var rules = c.GetExports<IDifferenceRule>();

            return rules.Select(r => new CompatibilityRule(r.GetType().Name)).ToList();
        }

        private static CompositionHost GetCompositionHost()
        {
            var configuration = new ContainerConfiguration().WithAssembly(typeof(CciAssemblyCompatibilityAnalyzer).GetTypeInfo().Assembly);
            return configuration.CreateContainer();
        }

        private static BaselineDifferenceFilter GetBaselineDifferenceFilter()
        {
            // TODO: allow baseline
            return null;
        }

        private static ICciComparers GetComparers()
        {
            if (!string.IsNullOrEmpty(s_remapFile))
            {
                if (!File.Exists(s_remapFile))
                {
                    throw new FileNotFoundException("ERROR: RemapFile {0} was not found!", s_remapFile);
                }
                return new NamespaceRemappingComparers(s_remapFile);
            }
            return CciComparers.Default;
        }

        private static ICciFilter GetCciFilter(bool enforcingMdilRules, bool excludeNonBrowsable)
        {
            if (enforcingMdilRules)
            {
                return new MdilPublicOnlyCciFilter()
                {
                    IncludeForwardedTypes = true
                };
            }
            else if (excludeNonBrowsable)
            {
                return new PublicEditorBrowsableOnlyCciFilter()
                {
                    IncludeForwardedTypes = true
                };
            }
            else
            {
                return new PublicOnlyCciFilter()
                {
                    IncludeForwardedTypes = true
                };
            }
        }

        private static DifferenceTracker GetDifferenceWriter(TextWriter writer, IDifferenceFilter filter)
        {
            var container = GetCompositionHost();

            bool ruleFilter(IDifferenceRuleMetadata ruleMetadata)
            {
                if (ruleMetadata.MdilServicingRule && !s_mdil)
                    return false;
                return true;
            };

            if (s_mdil && s_excludeNonBrowsable)
            {
                Trace.TraceWarning("Enforcing MDIL servicing rules and exclusion of non-browsable types are both enabled, but they are not compatible so non-browsable types will not be excluded.");
            }

            var cciFilter = GetCciFilter(s_mdil, s_excludeNonBrowsable);
            var settings = new MappingSettings
            {
                Comparers = GetComparers(),
                Filter = cciFilter,
                DiffFilter = new MappingDifferenceFilter(d => d != DifferenceType.Unchanged, cciFilter),
                DiffFactory = new ElementDifferenceFactory(container, ruleFilter),
                GroupByAssembly = s_groupByAssembly,
                IncludeForwardedTypes = true
            };

            if (filter == null)
            {
                filter = new DifferenceFilter<IncompatibleDifference>();
            }

            var diffWriter = new DifferenceTracker(settings, filter);
            ExportCciSettings.StaticSettings = settings.TypeComparer;

            return diffWriter;
        }

        private string GetFramework(IEnumerable<IFile> files)
        {
            var options = files.Select(f =>
            {
                var split = f.Path.Split(new[] { '/' });

                return split[1];
            }).Distinct();

            return options.Single();
        }

        // TODO: Clean up these errors
#pragma warning disable CS0169
#pragma warning disable CS0649
        private static string s_contractCoreAssembly;
        private static string s_contractSet;
        private static string s_implDirs;
        private static string s_contractLibDirs;
        private static bool s_listRules;
        private static string s_outFile;
        private static string s_baselineFileName;
        private static string s_remapFile;
        private static bool s_groupByAssembly = true;
        private static bool s_mdil;
        private static bool s_resolveFx;
        private static bool s_unifyToLibPaths = true;
        private static bool s_warnOnIncorrectVersion;
        private static bool s_ignoreDesignTimeFacades;
        private static bool s_excludeNonBrowsable;
        private static bool s_warnOnMissingAssemblies;
#pragma warning restore CS0169
#pragma warning restore CS0649
    }
}
