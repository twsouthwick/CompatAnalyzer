using System;
using System.CommandLine;

namespace CompatibilityAnalyzer
{
    internal class AnalysisOptionsBuilder : IAnalysisOptions, IReferenceAssemblyOptions
    {
        private string _packageId = null;
        private string _originalVersion = null;
        private string _updatedVersion = null;
        private string _feed = @"https://api.nuget.org/v3/index.json";
        private bool _verbose = false;
        private AnalysisCommand _command = AnalysisCommand.None;
        private string _referencePath;

        private AnalysisOptionsBuilder()
        {
        }

        public string Feed => _feed;

        public bool Verbose => _verbose;

        public string PackageName => _packageId;

        public string OriginalVersion => _originalVersion;

        public string UpdatedVersion => _updatedVersion;

        public string ReferencePath => _referencePath;

        public AnalysisCommand Command => _command;

        bool IReferenceAssemblyOptions.Create => _command == AnalysisCommand.CollectReferenceAssemblies;

        public static AnalysisOptionsBuilder Parse(string[] args)
        {
            var options = new AnalysisOptionsBuilder();
            var syntax = default(ArgumentSyntax);

            try
            {
                ArgumentSyntax.Parse(args, arg =>
                {
                    arg.HandleErrors = false;

                    arg.DefineCommand("analyze", ref options._command, AnalysisCommand.Analyze, "Analyze packages");

                    arg.DefineOption("packageId", ref options._packageId, true, "NuGet package to analyze");
                    arg.DefineOption("original", ref options._originalVersion, true, "Original package");
                    arg.DefineOption("updated", ref options._updatedVersion, true, "Updated packages");
                    arg.DefineOption("feed", ref options._feed, false, "NuGet feed to use");
                    arg.DefineOption("verbose", ref options._verbose, false, "Turn verbose output on");
                    arg.DefineOption("ref", ref options._referencePath, false, "Zip archive of reference assemblies");

                    arg.DefineCommand("ref", ref options._command, AnalysisCommand.CollectReferenceAssemblies, "Collect reference assemblies");
                    arg.DefineOption("path", ref options._referencePath, true, "Path to save reference assembly collection");

                    arg.DefineCommand("monitor", ref options._command, AnalysisCommand.MonitorQueue, "Monitor queue");

                    syntax = arg;
                });

                options.Validate(syntax);

                return options;
            }
            catch (ArgumentSyntaxException e)
            {
                throw new ArgumentParsingException(syntax, e.Message);
            }
        }

        private void Validate(ArgumentSyntax syntax)
        {
            switch (Command)
            {
                case AnalysisCommand.Analyze:
                    ValidateAnalyze(syntax);
                    break;
                case AnalysisCommand.CollectReferenceAssemblies:
                    ValidateReferenceAssemblyCollection(syntax);
                    break;
            }
        }

        private void ValidateReferenceAssemblyCollection(ArgumentSyntax syntax)
        {
            if (string.IsNullOrWhiteSpace(_referencePath))
            {
                throw new ArgumentParsingException(syntax, "Must supply a path to save output");
            }
        }

        private void ValidateAnalyze(ArgumentSyntax syntax)
        {
            if (string.IsNullOrWhiteSpace(_packageId))
            {
                throw new ArgumentParsingException(syntax, "Must supply a package ID");
            }

            if (string.IsNullOrWhiteSpace(_originalVersion))
            {
                throw new ArgumentParsingException(syntax, "Must supply a valid original version.");
            }

            if (string.IsNullOrWhiteSpace(_updatedVersion))
            {
                throw new ArgumentParsingException(syntax, "Must supply a valid updated version.");
            }

            if (string.IsNullOrWhiteSpace(_feed))
            {
                throw new ArgumentParsingException(syntax, "Feed cannot be empty");
            }
        }

        private class ArgumentParsingException : CompatibilityAnalysisException
        {
            private readonly ArgumentSyntax _syntax;

            /// <summary>
            /// Initializes a new instance of the <see cref="ArgumentParsingException"/> class.
            /// </summary>
            /// <param name="syntax">The argument syntax used to parse the command line</param>
            /// <param name="message">Exception message</param>
            public ArgumentParsingException(ArgumentSyntax syntax, string message)
                : base(message)
            {
                _syntax = syntax;
            }

            public override string Message => $"{base.Message}{Environment.NewLine}{Environment.NewLine}{_syntax.GetHelpText()}";
        }
    }
}
