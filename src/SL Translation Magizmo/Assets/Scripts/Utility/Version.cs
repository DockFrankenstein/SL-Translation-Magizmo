using qASIC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project
{
    [Serializable]
    public struct Version : IComparable<Version>
    {
        public Version(params uint[] releases)
        {
            this.releases = releases;
        }

        public uint[] releases;

        public static bool operator >(Version a, Version b)
        {
            var minLength = Mathf.Max(a.releases.Length, b.releases.Length);
            for (int i = 0; i < minLength; i++)
            {
                var aVal = i < a.releases.Length ? a.releases[i] : 0;
                var bVal = i < b.releases.Length ? b.releases[i] : 0;

                if (aVal > bVal) return true;
                if (aVal < bVal) return false;
            }

            return false;
        }

        public static bool operator <(Version a, Version b)
        {
            var minLength = Mathf.Max(a.releases.Length, b.releases.Length);
            for (int i = 0; i < minLength; i++)
            {
                var aVal = i < a.releases.Length ? a.releases[i] : 0;
                var bVal = i < b.releases.Length ? b.releases[i] : 0;

                if (aVal < bVal) return true;
                if (aVal > bVal) return false;
            }

            return false;
        }

        public static bool operator >=(Version a, Version b) =>
            a > b || a == b;

        public static bool operator <=(Version a, Version b) =>
            a < b || a == b;

        public static bool operator ==(Version a, Version b) =>
            (a.Equals(null) && b.Equals(null)) ||
                Enumerable.SequenceEqual(a.releases, b.releases);

        public static bool operator !=(Version a, Version b) =>
            !(a == b);

        public override bool Equals(object obj)
        {
            return obj is Version version &&
                   EqualityComparer<uint[]>.Default.Equals(releases, version.releases);
        }

        public override int GetHashCode() =>
            releases.GetHashCode();

        public override string ToString() =>
            releases != null ? string.Join(".", releases) : "NULL";

        public static bool TryParse(string s, out Version version)
        {
            version = new Version();

            var releaseStrings = s.Split('.');

            uint[] releases = new uint[releaseStrings.Length];

            for (int i = 0; i < releaseStrings.Length; i++)
            {
                if (!uint.TryParse(releaseStrings[i], out uint v))
                    return false;

                releases[i] = v;
            }

            version = new Version(releases);
            return true;
        }

        public int CompareTo(Version other)
        {
            if (this < other)
                return -1;

            if (this > other)
                return 1;

            return 0;
        }
    }
}