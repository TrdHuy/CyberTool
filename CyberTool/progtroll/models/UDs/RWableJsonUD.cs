using progtroll.models.VOs;
using System.Collections.Generic;

namespace progtroll.models.UDs
{
    internal class RWableJsonUD
    {
        public Dictionary<string, ProjectVO> ImportProjects { get; set; }
        public string CurrentImportedProjectPath { get; set; }
        public List<ReleaseTemplateUD> ReleaseTemplateSource { get; set; }
        public bool IsEmpty { get; private set; }

        public RWableJsonUD()
        {
            ImportProjects = new Dictionary<string, ProjectVO>();
            CurrentImportedProjectPath = "";
            ReleaseTemplateSource = new List<ReleaseTemplateUD>();
            IsEmpty = true;
        }

        public RWableJsonUD(Dictionary<string, ProjectVO> importProject
            , string currentImportedProjectPath
            , List<ReleaseTemplateUD> releaseTemplateItemSource)
        {
            ImportProjects = importProject;
            CurrentImportedProjectPath = currentImportedProjectPath;
            ReleaseTemplateSource = releaseTemplateItemSource;
        }
    }
}
