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
            = FeaturesParser.FeatureOrders["HUY.TD1_LOGGUARD_IS_SUPPORT_QUICK_FILTER_TEXT_BOX"];

        public static bool IS_SUPPORT_HIGH_CPU_LOG_CAPTURE { get; }
           = FeaturesParser.FeatureOrders["HUY.TD1_LOGGUARD_IS_SUPPORT_HIGH_CPU_LOG_CAPTURE"];

        public static bool IS_SUPPORT_DELETE_LOG_LINE { get; }
           = FeaturesParser.FeatureOrders["HUY.TD1_LOGGUARD_IS_SUPPORT_DELETE_LOG_LINE"];

        private sealed class FeaturesParser
        {
            private static string _titleGroup = @"(?<Title>\S+)";
            private static string _valueGroup = @"(?<Value>TRUE|FALSE)";
            private static string _pattern;

            public static Dictionary<string, bool> FeatureOrders = new Dictionary<string, bool>();

            static FeaturesParser()
            {
                FileParsing();
            }

            public static void FileParsing()
            {
                _pattern = _titleGroup + "=" + _valueGroup;

                string t = Properties.Resources.SecFloatingFeature;
                string[] t2 = t.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(line => !line.StartsWith("#")).ToArray();
                for (int i = 0; i < t2.Length; i++)
                {
                    Match match = Regex.Match(t2[i], _pattern);
                    if (match.Success)
                    {
                        FeatureOrders.Add(match.Groups["Title"].ToString(),
                            Convert.ToBoolean(match.Groups["Value"].ToString()));
                    }
                }

            }
        }
    }
}
