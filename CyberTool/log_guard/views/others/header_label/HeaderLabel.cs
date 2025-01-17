﻿using cyber_base.implement.command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace log_guard.views.others.header_label
{
    public class HeaderLabel : Label
    {
        private class HeaderLabelCmdImpl : ICommand
        {
            public event EventHandler CanExecuteChanged;
            private Action _act;
            public HeaderLabelCmdImpl(Action act)
            {
                _act = act;
            }
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                _act?.Invoke();
            }
        }

        #region MouseDoubleClickCommand
        public static readonly DependencyProperty MouseDoubleClickCommandProperty =
            DependencyProperty.Register("MouseDoubleClickCommand",
                typeof(ICommand),
                typeof(HeaderLabel),
                new PropertyMetadata(default(ICommand)));

        public ICommand MouseDoubleClickCommand
        {
            get
            {
                return (ICommand)GetValue(MouseDoubleClickCommandProperty);
            }
            set
            {
                SetValue(MouseDoubleClickCommandProperty, value);
            }
        }
        #endregion

        public HeaderLabel()
        {
            DefaultStyleKey = typeof(HeaderLabel);
        }

        private Border MainBorder;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            MainBorder = GetTemplateChild("MainBorder") as Border;

            MouseBinding OnLabelCmdMouseBinding = new MouseBinding();
            OnLabelCmdMouseBinding.MouseAction = MouseAction.LeftDoubleClick;
            OnLabelCmdMouseBinding.Command = new HeaderLabelCmdImpl(() =>
            {
                MouseDoubleClickCommand?.Execute(this);
            });

            MainBorder.InputBindings.Add(OnLabelCmdMouseBinding);
        }
    }
}
