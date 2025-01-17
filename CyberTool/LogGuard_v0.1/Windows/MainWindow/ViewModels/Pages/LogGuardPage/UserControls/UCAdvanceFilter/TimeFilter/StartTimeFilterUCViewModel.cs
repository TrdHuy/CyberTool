﻿using LogGuard_v0._1.Base.ViewModel;
using LogGuard_v0._1.Implement.UIEventHandler;
using LogGuard_v0._1.Windows.MainWindow.ViewModels.LogWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.ViewModels.Pages.LogGuardPage.UserControls.UCAdvanceFilter.TimeFilter
{
    public class StartTimeFilterUCViewModel : TimeFilterUCViewModel
    {
        public StartTimeFilterUCViewModel(BaseViewModel parent) : base(parent)
        {
        }

        public override bool Filter(object obj)
        {
            var data = obj as LWI_ParseableViewModel;
            if (!IsFilterEnable || FilterContent == "")
            {
                return true;
            }

            if(data != null)
            {
                return data.LogDateTime >= CurrentFilterTime;
            }
           
            
            return true;
        }
    }
}
