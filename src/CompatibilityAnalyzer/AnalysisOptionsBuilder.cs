using System;
using System.CommandLine;

namespace CompatibilityAnalyzer
{
    internal class AnalysisOptionsBuilder : IAnalysisOptions
    {
        private string _packageId = null;
        private string _originalVersion = null;
        private string _updatedVersion = null;
        private string _feed = @"https://api.nuget.org/v3/index.json";
        private bool _verbose = false;

        private AnalysisOptionsBuilder()
        {
        }

        public string Feed => _feed;

        public bool Verbose => _verbose;

        public string PackageName => _packageId;

        public string OriginalVersion => _originalVersion;

        public string UpdatedVersion => _updatedVersion;

        public static IAnalysisOptions Parse(string[] args)
        {
            var options = new AnalysisOptionsBuilder();

            var syntax = ArgumentSyntax.Parse(args, arg =>
            {
                arg.DefineOption("packageId", ref options._packageId, true, "NuGet package to analyze");
                arg.DefineOption("original", ref options._originalVersion, true, "Original package");
                arg.DefineOption("updated", ref options._updatedVersion, true, "Updated packages");
                arg.DefineOption("feed", ref options._feed, false, "NuGet feed to use");
                arg.DefineOption("verbose", ref options._verbose, false, "Turn verbose output on");
            });

            options.Validate(syntax);

            return options;
        }

        private void Validate(ArgumentSyntax syntax)
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
