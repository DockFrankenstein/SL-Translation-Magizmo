using System.Collections.Generic;
using System.Linq;

namespace Project
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> InsertBetween<T>(this IEnumerable<T> enumerable, T element)
        {
            return enumerable.SelectMany((x, i) => i == 0 ?
                new T[] { x } :
                new T[] { element, x });
        }
    }
}