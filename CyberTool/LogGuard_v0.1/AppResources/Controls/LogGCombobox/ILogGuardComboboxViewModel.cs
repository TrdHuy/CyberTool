using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogGuard_v0._1.AppResources.Controls.LogGCombobox
{
    public interface ILogGuardComboboxViewModel
    {
        ICommand OnComboBoxItemSelected { get; }
    }
}
