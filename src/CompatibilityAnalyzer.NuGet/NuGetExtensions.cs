using System;
using NuGet.Frameworks;

namespace CompatibilityAnalyzer
{
    internal static class NuGetExtensions
    {
        [Obsolete("TODO: Expose necessary members from INuGetFramework")]
        public static NuGetFramework AsFramework(this INuGetFramework framework)
        {
            if (framework is WrappedNuGetFramework wrapped)
            {
                return wrapped.Framework;
            }

            throw new InvalidOperationException($"{framework.GetType()} is not an instance of {nameof(NuGetFramework)}");
        }
    }
}