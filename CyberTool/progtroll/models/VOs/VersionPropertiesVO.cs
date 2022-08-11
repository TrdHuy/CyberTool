using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.models.VOs
{
    internal class VersionPropertiesVO
    {
        public static readonly string VERSION_MAJOR_PROPERTY_NAME = "major";
        public static readonly string VERSION_MINOR_PROPERTY_NAME = "minor";
        public static readonly string VERSION_PATCH_PROPERTY_NAME = "patch";
        public static readonly string VERSION_REVISION_PROPERTY_NAME = "revision";

        public string Major { get; set; } = "";
        public string Minor { get; set; } = "";
        public string Patch { get; set; } = "";
        public string Revision { get; set; } = "";
        public string Version { get; set; } = "";

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Major);
        }

        public override string ToString()
        {
            string res = "";
            if (Major != "")
            {
                res = Major;
                if (Minor != "")
                {
                    res += "." + Minor;
                }
                if (Patch != "")
                {
                    res += "." + Patch;
                }
                if (Revision != "")
                {
                    res += "." + Revision;
                }
            }
            return res;
        }

        public static bool operator <(VersionPropertiesVO t1, VersionPropertiesVO t2)
        {
            return new Version(Convert.ToInt32(string.IsNullOrEmpty(t1.Major) ? "0" : t1.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Minor) ? "0" : t1.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Patch) ? "0" : t1.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Revision) ? "0" : t1.Revision)) 
                < new Version(Convert.ToInt32(string.IsNullOrEmpty(t2.Major) ? "0" : t2.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Minor) ? "0" : t2.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Patch) ? "0" : t2.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Revision) ? "0" : t2.Revision));
        }

        public static bool operator <=(VersionPropertiesVO t1, VersionPropertiesVO t2)
        {
            return new Version(Convert.ToInt32(string.IsNullOrEmpty(t1.Major) ? "0" : t1.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Minor) ? "0" : t1.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Patch) ? "0" : t1.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Revision) ? "0" : t1.Revision))
                <= new Version(Convert.ToInt32(string.IsNullOrEmpty(t2.Major) ? "0" : t2.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Minor) ? "0" : t2.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Patch) ? "0" : t2.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Revision) ? "0" : t2.Revision));
        }

        public static bool operator >=(VersionPropertiesVO t1, VersionPropertiesVO t2)
        {
            return new Version(Convert.ToInt32(string.IsNullOrEmpty(t1.Major) ? "0" : t1.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Minor) ? "0" : t1.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Patch) ? "0" : t1.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Revision) ? "0" : t1.Revision))
                >= new Version(Convert.ToInt32(string.IsNullOrEmpty(t2.Major) ? "0" : t2.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Minor) ? "0" : t2.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Patch) ? "0" : t2.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Revision) ? "0" : t2.Revision));
        }

        public static bool operator >(VersionPropertiesVO t1, VersionPropertiesVO t2)
        {
            return new Version(Convert.ToInt32(string.IsNullOrEmpty(t1.Major) ? "0" : t1.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Minor) ? "0" : t1.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Patch) ? "0" : t1.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Revision) ? "0" : t1.Revision))
                > new Version(Convert.ToInt32(string.IsNullOrEmpty(t2.Major) ? "0" : t2.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Minor) ? "0" : t2.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Patch) ? "0" : t2.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Revision) ? "0" : t2.Revision));
        }

        public static bool operator ==(VersionPropertiesVO? t1, VersionPropertiesVO? t2)
        {
            if (!ReferenceEquals(t1, null) && !ReferenceEquals(t2, null))
                return new Version(Convert.ToInt32(string.IsNullOrEmpty(t1.Major) ? "0" : t1.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Minor) ? "0" : t1.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Patch) ? "0" : t1.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Revision) ? "0" : t1.Revision))
                == new Version(Convert.ToInt32(string.IsNullOrEmpty(t2.Major) ? "0" : t2.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Minor) ? "0" : t2.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Patch) ? "0" : t2.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Revision) ? "0" : t2.Revision));
            if (!ReferenceEquals(t1, null) && ReferenceEquals(t2, null))
                return false;
            if (ReferenceEquals(t1, null) && !ReferenceEquals(t2, null))
                return false;
            if (ReferenceEquals(t1, null) && ReferenceEquals(t2, null))
                return true;
            return false;
        }

        public static bool operator !=(VersionPropertiesVO? t1, VersionPropertiesVO? t2)
        {
            if (!ReferenceEquals(t1, null) && !ReferenceEquals(t2, null))
                return new Version(Convert.ToInt32(string.IsNullOrEmpty(t1.Major) ? "0" : t1.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Minor) ? "0" : t1.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Patch) ? "0" : t1.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t1.Revision) ? "0" : t1.Revision))
                != new Version(Convert.ToInt32(string.IsNullOrEmpty(t2.Major) ? "0" : t2.Major)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Minor) ? "0" : t2.Minor)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Patch) ? "0" : t2.Patch)
                , Convert.ToInt32(string.IsNullOrEmpty(t2.Revision) ? "0" : t2.Revision));
            if (!ReferenceEquals(t1, null) && ReferenceEquals(t2, null))
                return true;
            if (ReferenceEquals(t1, null) && !ReferenceEquals(t2, null))
                return true;
            if (ReferenceEquals(t1, null) && ReferenceEquals(t2, null))
                return false;
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return (VersionPropertiesVO)obj == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
