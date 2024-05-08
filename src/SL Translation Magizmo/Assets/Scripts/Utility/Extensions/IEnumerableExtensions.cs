﻿using System;
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

        /// <summary>Sorts a list using the search string</summary>
        /// <param name="list">List to sort</param>
        /// <param name="search">Search bar value</param>
        /// <returns>The sorted list</returns>
        public static IEnumerable<string> SortSearchList(this IEnumerable<string> list, string search) =>
            SortSearchList(list, x => x, search);

        /// <summary>Sorts a list using the search string</summary>
        /// <param name="list">List to sort</param>
        /// <param name="func">Select the string</param>
        /// <param name="search">Search bar value</param>
        /// <returns>The sorted list</returns>
        public static IEnumerable<T> SortSearchList<T>(this IEnumerable<T> list, Func<T, string> func, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return list;

            string[] keywords = search
                .ToLower()
                .Split(' ')
                .Where(s => s != string.Empty)
                .ToArray();

            return list
                .Select(x => new KeyValuePair<T, string>(x, func(x)))
                .GroupBy(x =>
                {
                    string s = x.Value.ToLower();
                    float percentage = 0f;
                    foreach (var keyword in keywords)
                    {
                        if (!s.Contains(keyword))
                            return 0f;

                        percentage += (float)s.Length / keyword.Length;
                        s = s.Replace(keyword, " ");
                    }

                    return percentage;
                })
                .Where(x => x.Key > 0f)
                .OrderBy(x => x.Key)
                .SelectMany(x => x)
                .Select(x => x.Key)
                .ToList();
        }
    }
}