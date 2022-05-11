﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace log_guard.flow.view_helper
{
    public class LogGuardViewHelper
    {
        private static Dictionary<LogGuardViewKeyDefinition, object> ViewMap;

        static LogGuardViewHelper()
        {
            ViewMap = new Dictionary<LogGuardViewKeyDefinition, object>();
        }

        #region ViewKey
        public static readonly DependencyProperty ViewKeyProperty = DependencyProperty.RegisterAttached(
                 "ViewKey",
                 typeof(LogGuardViewKeyDefinition),
                 typeof(LogGuardViewHelper),
                 new PropertyMetadata(default(LogGuardViewKeyDefinition), OnViewKeyChangedCallback));

        private static void OnViewKeyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var key = (LogGuardViewKeyDefinition)e.NewValue;
            if (ViewMap.ContainsKey(key))
            {
                ViewMap.Remove(key);
            }
            ViewMap.Add(key, d);
        }

        public static LogGuardViewKeyDefinition GetViewKey(UIElement obj)
        {
            return (LogGuardViewKeyDefinition)obj.GetValue(ViewKeyProperty);
        }

        public static void SetViewKey(UIElement obj, LogGuardViewKeyDefinition value)
        {
            obj.SetValue(ViewKeyProperty, value);
        }
        #endregion

        public static LogGuardViewHelper Current
        {
            get
            {
                return LogGuardSingletonController.LG_ViewHelper;
            }
        }

        public object GetViewByKey(LogGuardViewKeyDefinition key)
        {
            return ViewMap[key];
        }
    }

    public enum LogGuardViewKeyDefinition
    {
        LogWatcherViewer = 1,
        LogWatcherZoomButton = 2,
    }
}
