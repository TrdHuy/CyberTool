using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace log_guard.@base.control.combobox
{
    internal interface ILogGuardComboboxViewModel
    {
        ICommand OnComboBoxItemSelected { get; }
    }
}
