using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogGuard_v0._1._Config
{
    public static class RUNE
    {
        public static void Init()
        {
        }

        public static bool IS_SUPPORT_QUICK_FILTER_TEXT_BOX { get; }
            = FeaturesParser.BoolFeatureOrders["HUY.TD1_LOGGUARD_IS_SUPPORT_QUICK_FILTER_TEXT_BOX"];

        public static bool IS_SUPPORT_HIGH_CPU_LOG_CAPTURE { get; }
           = FeaturesParser.BoolFeatureOrders["HUY.TD1_LOGGUARD_IS_SUPPORT_HIGH_CPU_LOG_CAPTURE"];

        public static bool IS_SUPPORT_DELETE_LOG_LINE { get; }
           = FeaturesParser.BoolFeatureOrders["HUY.TD1_LOGGUARD_IS_SUPPORT_DELETE_LOG_LINE"];

        public static int MAXIMUM_TAG_ITEM { get; }
           = FeaturesParser.IntFeatureOrders["HUY.TD1_MAXIMUM_TAG_ITEMS_OF_TAG_MANAGER"];

        public static int MAXIMUM_MESSAGE_ITEM { get; }
          = FeaturesParser.IntFeatureOrders["HUY.TD1_MAXIMUM_TAG_ITEMS_OF_MESSAGE_MANAGER"];

        private sealed class FeaturesParser
        {
            private static string _titleGroup = @"(?<Title>\S+)";
            private static string _boolValueGroup = @"(?<Value>TRUE|FALSE)";
            private static string _intValueGroup = @"(?<Value>\d+)";
            private static string _pattern;

            public static Dictionary<string, bool> BoolFeatureOrders = new Dictionary<string, bool>();
            public static Dictionary<string, int> IntFeatureOrders = new Dictionary<string, int>();

            static FeaturesParser()
            {
                FileParsing();
            }

            public static void FileParsing()
            {
                //Bool sec feature parser
                _pattern = _titleGroup + "=" + _boolValueGroup;
                string t = Properties.Resources.BoolSecFloatingFeature;
                string[] t2 = t.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(line => !line.StartsWith("#")).ToArray();
                for (int i = 0; i < t2.Length; i++)
                {
                    Match match = Regex.Match(t2[i], _pattern);
                    if (match.Success)
                    {
                        BoolFeatureOrders.Add(match.Groups["Title"].ToString(),
                                Convert.ToBoolean(match.Groups["Value"].ToString()));
                    }

                }

                //Int sec feature parser
                _pattern = _titleGroup + "=" + _intValueGroup;
                t = Properties.Resources.IntSecFloatingFeature;
                t2 = t.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(line => !line.StartsWith("#")).ToArray();
                for (int i = 0; i < t2.Length; i++)
                {
                    Match match = Regex.Match(t2[i], _pattern);
                    if (match.Success)
                    {
                        IntFeatureOrders.Add(match.Groups["Title"].ToString(),
                                Convert.ToInt32(match.Groups["Value"].ToString()));
                    }

                }
            }
        }
    }
}
