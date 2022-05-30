using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.view_models.project_manager.items
{
    internal class VersionHistoryItemViewModel : BaseViewModel
    {
        private bool _isFirst;
        private bool _isLast;

        [Bindable(true)]
        public bool IsFirst
        {
            get
            {
                return _isFirst;
            }
            set
            {
                _isFirst = value;
                InvalidateOwn();
            }
        }


        [Bindable(true)]
        public bool IsLast
        {
            get
            {
                return _isLast;
            }
            set
            {
                _isLast = value;
                InvalidateOwn();
            }
        }

        public VersionHistoryItemViewModel()
        {

        }
    }
}
