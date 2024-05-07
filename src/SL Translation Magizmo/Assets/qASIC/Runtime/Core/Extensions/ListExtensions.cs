using System.Collections.Generic;
using System;

namespace qASIC
{
    public static class ListExtensions
    {
        public static IList<TSource> Where<TSource>(this IList<TSource> source, Func<int, TSource, bool> func)
        {
            IList<TSource> result = new List<TSource>();
            int count = source.Count;
            for (int i = 0; i < count; i++)
            {
                var item = source[i];
                if (func?.Invoke(i, item) == true)
                    result.Add(item);
            }

            return result;
        }
    }
}