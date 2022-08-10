using cyber_base.implement.utils;
using honeyboard_release_service.definitions;
using honeyboard_release_service.implement.module;
using honeyboard_release_service.models.VOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.implement.project_manager.version_parser
{
    internal class VersionAttributeParsingManager
    {
        private const string PARSER_INFO_DATA_FILE_NAME = "\\version_attribute_parsers.json";
        private readonly string PARSER_INFO_DATA_FILE_PATH = PublisherDefinition.PLUGIN_BASE_FOLDER_PATH
            + PARSER_INFO_DATA_FILE_NAME;

        private VersionAttributeParser _parser;
        private JSONVersionAttributeParserInformation? _parserInformation;
        private string _versionAttrFileContent = "";
        private Dictionary<string, string[]> _syntaxMap;
        public VersionAttributeParsingManager()
        {
            _syntaxMap = new Dictionary<string, string[]>();
            _parser = new VersionAttributeParser();
        }

        public async void LoadParserInformationFromFile()
        {
            if (!Directory.Exists(PublisherDefinition.PLUGIN_BASE_FOLDER_PATH))
            {
                Directory.CreateDirectory(PublisherDefinition.PLUGIN_BASE_FOLDER_PATH);
            }
            if (!File.Exists(PARSER_INFO_DATA_FILE_PATH))
            {
                File.Create(PARSER_INFO_DATA_FILE_PATH).Dispose();
            }
            var jsonString = await File.ReadAllTextAsync(PARSER_INFO_DATA_FILE_PATH);

            if (string.IsNullOrEmpty(jsonString))
            {
                jsonString = Properties.Resources.default_parser_information;
            }

            _parserInformation = JsonHelper.DeserializeObject<JSONVersionAttributeParserInformation>(jsonString);

            if (_parserInformation?.SyntaxArr != null)
            {
                _syntaxMap.Clear();
                foreach(var syntax in _parserInformation.SyntaxArr)
                {
                    _syntaxMap.Add(syntax.MainSyntax, syntax.SubSyntaxs);
                }
            }
        }

        public string ModifyVersionAttributeOfOriginText(Dictionary<string, string> atrributesMap)
        {
            return _parser.ModifyVersionAttributeOfOriginText(_versionAttrFileContent, atrributesMap);
        }

        public VersionPropertiesVO GetVersionPropertiesFromVersionFileContent(string content = "")
        {
            if (string.IsNullOrEmpty(content))
            {
                return _parser.GetVersionPropertiesFromOriginText(_versionAttrFileContent);
            }
            return _parser.GetVersionPropertiesFromOriginText(content);
        }

        public void SetVersionAttrFileContent(string content)
        {
            _versionAttrFileContent = content;
        }

        public void SetCurrentParserSyntax(string mainSyntax)
        {
            if (_syntaxMap.ContainsKey(mainSyntax))
            {
                _parser.SetVersionAttributeParserSyntax(mainSyntax, _syntaxMap[mainSyntax]);
            }
        }

        public string[]? GetVersionPropertiesFileName()
        {
            return _parserInformation?.FileNameArr;
        }

        public string[]? GetVersionPropertiesParserMainSyntax()
        {
            return _syntaxMap.Keys.ToArray();
        }
    }
}
