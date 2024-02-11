using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [Serializable]
    public struct Version
    {
        public Version(params uint[] releases)
        {
            this.releases = releases;
        }

        public uint[] releases;

        public static bool operator >(Version a, Version b)
        {
            var minLength = Mathf.Min(a.releases.Length, b.releases.Length);
            for (int i = 0; i < minLength; i++)
            {
                if (a.releases[i] > b.releases[i]) return true;
                if (a.releases[i] < b.releases[i]) return false;
            }

            return a.releases.Length > b.releases.Length;
        }

        public static bool operator <(Version a, Version b)
        {
            var minLength = Mathf.Min(a.releases.Length, b.releases.Length);
            for (int i = 0; i < minLength; i++)
            {
                if (a.releases[i] < b.releases[i]) return true;
                if (a.releases[i] > b.releases[i]) return false;
            }

            return a.releases.Length < b.releases.Length;
        }

        public static bool operator >=(Version a, Version b) =>
            a > b || a == b;

        public static bool operator <=(Version a, Version b) =>
            a < b || a == b;

        public static bool operator ==(Version a, Version b) =>
            a.releases == b.releases;

        public static bool operator !=(Version a, Version b) =>
            a.releases != b.releases;

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
    }
}