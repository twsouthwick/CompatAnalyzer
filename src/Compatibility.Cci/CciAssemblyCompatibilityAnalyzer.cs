using ApiCompat;
using Microsoft.Cci;
using Microsoft.Cci.Comparers;
using Microsoft.Cci.Differs;
using Microsoft.Cci.Extensions;
using Microsoft.Cci.Filters;
using Microsoft.Cci.Mappings;
using Microsoft.Cci.Writers;
using System;
using System.Collections.Generic;
using System.Composition;
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
        private readonly string _reference;

        public CciAssemblyCompatibilityAnalyzer(TextWriter log)
        {
            _log = log;
            _reference = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7";

            Trace.Listeners.Add(new TextWriterTraceListener(_log)
            {
                Filter = new EventTypeFilter(SourceLevels.Error | SourceLevels.Warning)
            });
        }

        private HostEnvironment CreateHostEnvironment(NameTable nameTable)
        {
            var host = new HostEnvironment(nameTable)
            {
                ResolveAgainstRunningFramework = true,
                UnifyToLibPath = s_unifyToLibPaths,
                LoadErrorTreatment = ErrorTreatment.TreatAsWarning
            };

            host.UnableToResolve += new EventHandler<UnresolvedReference<IUnit, AssemblyIdentity>>(implHost_UnableToResolve);
            host.AddLibPath(_reference);

            return host;
        }

        public void Analyze(IEnumerable<IAssemblyFile> version1Assemblies, IEnumerable<IAssemblyFile> version2Assemblies)
        {
            var filter = GetBaselineDifferenceFilter();
            var sharedNameTable = new NameTable();

            var contractHost = CreateHostEnvironment(sharedNameTable);
            var contractAssemblies = contractHost.LoadAssemblies(version1Assemblies);

            if (s_ignoreDesignTimeFacades)
                contractAssemblies = contractAssemblies.Where(a => !a.IsFacade());

            var implHost = CreateHostEnvironment(sharedNameTable);
            var implAssemblies = implHost.LoadAssemblies(version2Assemblies);

            // Exit after loading if the code is set to non-zero
            if (DifferenceWriter.ExitCode != 0)
                return;

            var writer = GetDifferenceWriter(_log, filter);
            writer.Write(s_implDirs, implAssemblies, s_contractSet, contractAssemblies);
        }

        public IEnumerable<string> GetRules()
        {
            var c = GetCompositionHost();
            var rules = c.GetExports<IDifferenceRule>();

            return rules.Select(r => r.GetType().Name).ToList();
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

        private static void implHost_UnableToResolve(object sender, UnresolvedReference<IUnit, AssemblyIdentity> e)
        {
            Trace.TraceError("Unable to resolve assembly '{0}' referenced by the implementation assembly '{1}'.", e.Unresolved, e.Referrer);
        }

        private static void contractHost_UnableToResolve(object sender, UnresolvedReference<IUnit, AssemblyIdentity> e)
        {
            Trace.TraceError("Unable to resolve assembly '{0}' referenced by the contract assembly '{1}'.", e.Unresolved, e.Referrer);
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

        private static ICciDifferenceWriter GetDifferenceWriter(TextWriter writer, IDifferenceFilter filter)
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

            ICciDifferenceWriter diffWriter = new DifferenceWriter(writer, settings, filter);
            ExportCciSettings.StaticSettings = settings.TypeComparer;

            // Always compose the diff writer to allow it to import or provide exports
            container.SatisfyImports(diffWriter);

            return diffWriter;
        }

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
    }
}
