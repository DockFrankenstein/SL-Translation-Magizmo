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
            return items;
        }

        public static string ToEntryContent(this IEnumerable<string> array) =>
            string.Join("\n", array);

        public static IEnumerable<string> SplitWithSplits(this IEnumerable<string> array, string split, StringSplitOptions splitOptions = StringSplitOptions.None) =>
            array.SelectMany(x => x.Split(split, splitOptions).InsertBetween(split))
            .Where(x => !string.IsNullOrEmpty(x));

        public static string[] SplitByLines(this string s) =>
            s.Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n');
    }
}