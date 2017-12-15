using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Viking.Deployment
{
    public struct Version : IComparable<Version>
    {
        private int[] Parts { get; }
        public int this[int part] => Parts[part];
        public int NumParts => Parts.Length;

        public Version(string version) : this(version.Split('.').Select(a => int.Parse(a, NumberFormatInfo.InvariantInfo))) { }
        public Version(IEnumerable<int> e) : this(e.ToArray()) { }
        public Version(params int[] parts)
        {
            Parts = new int[parts.Length];
            parts.CopyTo(Parts, 0);
        }

        public override string ToString() => string.Join(".", Parts);
        public override int GetHashCode() => Parts.Sum(a => a.GetHashCode()).GetHashCode();
        public override bool Equals(object obj)
        {
            var ver = obj as Version?;
            if (ver == null) return false;
            return ver == this;
        }

        public int CompareTo(Version other)
        {
            var min = Math.Min(other.NumParts, NumParts);
            for(int i = 0; i < min; ++i)
            {
                var cmp = this[i].CompareTo(other[i]);
                if (cmp == 0)
                    continue;
                return cmp;
            }
            if (other.NumParts == NumParts)
                return 0;
            return other.NumParts < NumParts ? 1 : -1;
        }

        public static bool operator<(Version a, Version b) => a.CompareTo(b) < 0;
        public static bool operator>(Version a, Version b) => a.CompareTo(b) > 0;
        public static bool operator<=(Version a, Version b) => a.CompareTo(b) <= 0;
        public static bool operator>=(Version a, Version b) => a.CompareTo(b) >= 0;
        public static bool operator==(Version a, Version b) => a.CompareTo(b) == 0;
        public static bool operator!=(Version a, Version b) => a.CompareTo(b) != 0;
    }
}
