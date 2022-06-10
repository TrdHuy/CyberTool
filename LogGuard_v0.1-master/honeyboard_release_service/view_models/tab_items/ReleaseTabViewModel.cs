using cyber_base.view_model;
using honeyboard_release_service.view_models.command.tab_items.release_tab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.tab_items
{
    internal class ReleaseTabViewModel : BaseViewModel
    {
        private static readonly Regex PLMCaseCodeRegex = new Regex(@"P([0-9]{6})-([0-9]{5})");
        private string _taskID = "";
        private string _commitTitle = "";
        private string _commitDescription = "";
        private string _major = "";
        private string _minor = "";
        private string _patch = "";
        private string _revision = "";

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
                return _major;
            }
            set
            {
                _major = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Minor
        {
            get
            {
                return _minor;
            }
            set
            {
                _minor = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Patch
        {
            get
            {
                return _patch;
            }
            set
            {
                _patch = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Revision
        {
            get
            {
                return _revision;
            }
            set
            {
                _revision = value;
                InvalidateOwn();
            }
        }

        public ReleaseTabViewModel(BaseViewModel parents)
        {
            ButtonCommandVM = new RT_ButtonCommandVM(this);
        }
    }
}
