using System;
using System.Collections.Generic;
using System.Text;

namespace CompatibilityAnalyzer
{
    public static class CollectionsExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) => new HashSet<T>(source, comparer);
    }
}
