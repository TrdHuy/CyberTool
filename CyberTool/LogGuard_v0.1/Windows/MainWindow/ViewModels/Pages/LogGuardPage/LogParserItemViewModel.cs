using LogGuard_v0._1.AppResources.Controls.LogGCombobox;
using LogGuard_v0._1.Base.Command;
using LogGuard_v0._1.Base.LogGuardFlow;
using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage
{
    public class LogParserItemViewModel : BaseViewModel, ILogGuardComboboxViewModel
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
