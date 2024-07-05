using System.Collections.Generic;
using System.Linq;
using System;

namespace qASIC
{
    public static class IEnumerableExtensions
    {
        public static bool IndexInRange<T>(this IEnumerable<T> list, int index) =>
            index >= 0 && index < list.Count();

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector)
        {
            List<TSource> list = source.ToList();
            var targets = list.Where(x => selector.Invoke(x));

            if (targets.Count() != 1)
                return -1;

            return list.IndexOf(targets.First());
        }
    }
}