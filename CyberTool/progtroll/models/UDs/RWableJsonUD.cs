using progtroll.models.VOs;
using progtroll.view_models.tab_items;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace progtroll.models.UDs
{
    internal class RWableJsonUD
    {
        public Dictionary<string, ProjectVO> ImportProjects { get; set; }
        public string CurrentImportedProjectPath { get; set; }
        public List<ReleaseTemplateUD> ReleaseTemplateSource { get; set; }

        public RWableJsonUD()
        {
            ImportProjects = new Dictionary<string, ProjectVO>();
            CurrentImportedProjectPath = "";
            ReleaseTemplateSource = new List<ReleaseTemplateUD>();
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
