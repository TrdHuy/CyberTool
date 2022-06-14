using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.models.VOs
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
    }
}
