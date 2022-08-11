using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_core.windows.cyber_istand.view_models
{
    internal class CyberIStandWindowViewModel : BaseViewModel
    {
        private string _title = "Please wait!";
        private string _content = "Resource not available!";
        private double _totalPercent = 0d;
        private double _currentTaskPercent = 0d;
        private string _currentTaskPercentToString = "0%";

        [Bindable(true)]
        public double TotalPercent
        {
            get
            {
                return _totalPercent;
            }
            set
            {

                _totalPercent = value;
                InvalidateOwn();
            }
        }

        [Bindable(true)]
        public string CurrentTaskPercentToString
        {
            get
            {
                return _currentTaskPercentToString;
            }
        }

        [Bindable(true)]
        public double CurrentTaskPercent
        {
            get
            {
                return _currentTaskPercent;
            }
            set
            {
                _currentTaskPercentToString = Math.Round(value, 2) + "%";
                _currentTaskPercent = value;
                InvalidateOwn();
                Invalidate("CurrentTaskPercentToString");
            }
        }

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
