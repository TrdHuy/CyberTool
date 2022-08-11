using cyber_base.view_model;
using progtroll.definitions;
using progtroll.models.VOs;
using progtroll.view_models.command.tab_items.release_tab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace progtroll.view_models.tab_items
{
    internal class ReleaseTabViewModel : BaseViewModel
    {
        private static readonly Regex PLMCaseCodeRegex = new Regex(PublisherDefinition.PLM_CASE_CODE_REGEX);
        private string _taskID = "";
        private string _commitTitle = "";
        private string _commitDescription = "";
        private VersionPropertiesVO _modifiedVersionPropVO;

        public VersionPropertiesVO ModifiedVersionPropVO
        {
            get
            {
                return _modifiedVersionPropVO;
            }
        }

        [Bindable(true)]
        public RT_ButtonCommandVM ButtonCommandVM { get; set; }

        [Bindable(true)]
        public string TaskIDUri
        {
            get
            {
                if (PLMCaseCodeRegex.IsMatch(_taskID))
                {
                    return "http://splm.sec.samsung.net/wl/com/ozSearch/searchMain.do?searchWord=" + _taskID;
                }
                else
                {
                    return "https://mobilerndhub.sec.samsung.net/its/browse/" + _taskID;
                }
            }

        }

        [Bindable(true)]
        public string TaskID
        {
            get
            {
                return _taskID;
            }
            set
            {
                _taskID = value;
                InvalidateOwn();
                Invalidate("TaskIDUri");
            }
        }

        [Bindable(true)]
        public string CommitTitle
        {
            get
            {
                return _commitTitle;
            }
            set
            {
                _commitTitle = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string CommitDescription
        {
            get
            {
                return _commitDescription;
            }
            set
            {
                _commitDescription = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Major
        {
            get
            {
                return _modifiedVersionPropVO.Major;
            }
            set
            {
                _modifiedVersionPropVO.Major = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Minor
        {
            get
            {
                return _modifiedVersionPropVO.Minor;
            }
            set
            {
                _modifiedVersionPropVO.Minor = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Patch
        {
            get
            {
                return _modifiedVersionPropVO.Patch;
            }
            set
            {
                _modifiedVersionPropVO.Patch = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Revision
        {
            get
            {
                return _modifiedVersionPropVO.Revision;
            }
            set
            {
                _modifiedVersionPropVO.Revision = value;
                InvalidateOwn();
            }
        }

        public ReleaseTabViewModel(BaseViewModel parents) : base(parents)
        {
            ButtonCommandVM = new RT_ButtonCommandVM(this);
            _modifiedVersionPropVO = new VersionPropertiesVO();
        }
    }
}
