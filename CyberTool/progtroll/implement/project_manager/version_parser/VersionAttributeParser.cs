using progtroll.models.VOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace progtroll.implement.project_manager.version_parser
{
    internal class VersionAttributeParser
    {
        private const string MAJOR_KEY = "major";
        private const string MINOR_KEY = "minor";
        private const string PATCH_KEY = "patch";
        private const string REVISION_KEY = "revision";

        private string _versionAttributeParserMainSyntax = "";
        private string[]? _versionAttributeParserSubSyntax;


        public void SetVersionAttributeParserSyntax(string mainSyntax, string[] subSyntaxs)
        {
            _versionAttributeParserMainSyntax = mainSyntax;
            _versionAttributeParserSubSyntax = subSyntaxs;
        }

        public string ModifyVersionAttributeOfOriginText(string origin, Dictionary<string, string> atrributesMap)
        {
            if (!string.IsNullOrEmpty(_versionAttributeParserMainSyntax))
            {
                var newText = origin;
                var mainMatch = Regex.Match(origin, _versionAttributeParserMainSyntax);
                if (mainMatch.Success)
                {
                    foreach (var kv in atrributesMap)
                    {
                        string key = kv.Key;
                        string value = kv.Value;
                        if (mainMatch.Groups.ContainsKey(key))
                        {
                            var regex = new Regex(_versionAttributeParserMainSyntax);
                            newText = ReplaceGroup(regex, newText, key, value);
                        }
                    }
                    return newText;
                }
                else if (_versionAttributeParserSubSyntax != null)
                {
                    foreach (var syntax in _versionAttributeParserSubSyntax)
                    {
                        var subMatch = Regex.Match(origin, syntax);
                        if (subMatch.Success)
                        {
                            foreach (var kv in atrributesMap)
                            {
                                string key = kv.Key;
                                string value = kv.Value;
                                if (mainMatch.Groups.ContainsKey(key))
                                {
                                    var regex = new Regex(syntax);
                                    newText = ReplaceGroup(regex, newText, key, value);
                                }
                            }
                            return newText;
                        }
                    }
                }
            }
            return origin;
        }

        public VersionPropertiesVO GetVersionPropertiesFromOriginText(string origin)
        {
            var mainMatch = Regex.Match(origin, _versionAttributeParserMainSyntax);
            if (mainMatch.Success)
            {
                return GetVersionPropFromMatch(mainMatch);
            }
            else if (_versionAttributeParserSubSyntax != null)
            {
                foreach (var syntax in _versionAttributeParserSubSyntax)
                {
                    var subMatch = Regex.Match(origin, syntax);
                    if (subMatch.Success)
                    {
                        return GetVersionPropFromMatch(subMatch);
                    }
                }
            }
            return new VersionPropertiesVO();
        }

        private static VersionPropertiesVO GetVersionPropFromMatch(Match match)
        {
            VersionPropertiesVO vp = new VersionPropertiesVO();
            if (match.Success)
            {
                if (match.Groups.ContainsKey(MAJOR_KEY))
                {
                    vp.Major = match.Groups[MAJOR_KEY].Value;
                }
                if (match.Groups.ContainsKey(MINOR_KEY))
                {
                    vp.Minor = match.Groups[MINOR_KEY].Value;
                }
                if (match.Groups.ContainsKey(REVISION_KEY))
                {
                    vp.Revision = match.Groups[REVISION_KEY].Value;
                }
                if (match.Groups.ContainsKey(PATCH_KEY))
                {
                    vp.Patch = match.Groups[PATCH_KEY].Value;
                }
            }
            return vp;
        }
        private static string ReplaceGroup(Regex regex, string input, string groupName, string replacement)
        {
            return regex.Replace(
                input,
                m =>
                {
                    var group = m.Groups[groupName];
                    var sb = new StringBuilder();
                    var previousCaptureEnd = 0;
                    foreach (var capture in group.Captures.Cast<Capture>())
                    {
                        var currentCaptureEnd =
                            capture.Index + capture.Length - m.Index;
                        var currentCaptureLength =
                            capture.Index - m.Index - previousCaptureEnd;
                        sb.Append(
                            m.Value.Substring(
                                previousCaptureEnd, currentCaptureLength));
                        sb.Append(replacement);
                        previousCaptureEnd = currentCaptureEnd;
                    }
                    sb.Append(m.Value.Substring(previousCaptureEnd));

                    return sb.ToString();
                });
        }
    }
}
