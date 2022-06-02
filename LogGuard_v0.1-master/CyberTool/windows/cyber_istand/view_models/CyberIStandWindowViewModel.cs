using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_tool.windows.cyber_istand.view_models
{
    internal class CyberIStandWindowViewModel : BaseViewModel
    {
        private string _title = "Please wait!";
        private string _content = "Resource not available!";

        [Bindable(true)]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {

                _title = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {

                _content = value;
                InvalidateOwn();
            }
        }
    }
}
