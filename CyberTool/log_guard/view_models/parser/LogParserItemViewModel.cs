using cyber_base.view_model;
using log_guard.@base.control.combobox;
using log_guard.models.vo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace log_guard.view_models.parser
{
    internal class LogParserItemViewModel : BaseViewModel, ILogGuardComboboxViewModel
    {
        private LogParserVO _logParserVO;

        [Bindable(true)]
        public string DisplayName
        {
            get
            {
                return _logParserVO.DisplayName;
            }
        }

        [Bindable(true)]
        public string ParserTip
        {
            get
            {
                return _logParserVO.ParserTip;
            }
        }

        [Bindable(true)]
        public ICommand OnComboBoxItemSelected { get; set; }

        public LogParserVO ParserVO { get { return _logParserVO; } }

        public LogParserItemViewModel(LogParserVO vo)
        {
            _logParserVO = vo;
        }

    }
}

