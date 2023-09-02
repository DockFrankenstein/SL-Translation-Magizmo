using System;
using System.Linq;
using System.Collections.Generic;

namespace Project
{
    public static partial class StringExtensions
    {
        public static string[] EntryContentToArray(this string entry)
        {
            var items = entry.Split('\n');
            Array.Resize(ref items, items.Length - 1);
            return items;
        }

        public static string ToEntryContent(this IEnumerable<string> array) =>
            string.Join(string.Empty, array
            .Select(x => $"{x}\n"));
    }
}