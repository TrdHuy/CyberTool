using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.implement.project_manager.version_parser
{
    internal class JSONVersionAttributeParserInformation
    {
        public class ParserInformation
        {
            public string MainSyntax { get; set; }
            public string[] SubSyntaxs { get; set; }

            public ParserInformation(string mainSyntax, string[] subSyntaxs)
            {
                MainSyntax = mainSyntax;
                SubSyntaxs = subSyntaxs;
            }
        }

        public string[] FileNameArr { get; set; }
        public ParserInformation[] SyntaxArr { get; set; }

        public JSONVersionAttributeParserInformation(string[] fileNameArr, ParserInformation[] syntaxsArr)
        {
            FileNameArr = fileNameArr;
            SyntaxArr = syntaxsArr;
        }
    }
}
