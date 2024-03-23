using System;
using System.Linq;
using System.Collections.Generic;

namespace Project
{
    public static partial class StringExtensions
    {
        public static string[] EntryContentToArray(this string entry)
        {
            var items = entry
                .Split('\n')
                .AsEnumerable();

            var last = items.Last();

            if (string.IsNullOrEmpty(last))
                items = items
                    .SkipLast(1);

            return items.ToArray();
        }

        public static string ToEntryContent(this IEnumerable<string> array)
        {
            if (array.Count() == 0)
                return string.Empty;

            var content = string.Join("\n", array);

            if (string.IsNullOrEmpty(array.Last()))
                content = $"{content}\n";

            return content;
        }

        public static string GetBaseId(this string id) =>
            id.Split(':').Last();

        public static int GetIdIndex(this string id)
        {
            var split = id.Split(':');
            if (split.Length > 1 && int.TryParse(split.Last(), out int index))
                return index;

            return 0;
        }

        public static IEnumerable<string> SplitWithSplits(this IEnumerable<string> array, string split, StringSplitOptions splitOptions = StringSplitOptions.None) =>
            array.SelectMany(x => x.Split(split, splitOptions).InsertBetween(split))
            .Where(x => !string.IsNullOrEmpty(x));

        public static string[] SplitByLines(this string s) =>
            s.Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n');
    }
}