using log_guard.@base.module;
using log_guard.definitions;
using log_guard.implement.module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace log_guard.implement.flow.view_helper
{
    public class LogGuardViewHelper : ILogGuardModule
    {
        private Dictionary<LogGuardViewKeyDefinition, object> ViewMap;

        #region ViewKey
        public static readonly DependencyProperty ViewKeyProperty = DependencyProperty.RegisterAttached(
                 "ViewKey",
                 typeof(LogGuardViewKeyDefinition),
                 typeof(LogGuardViewHelper),
                 new PropertyMetadata(default(LogGuardViewKeyDefinition), OnViewKeyChangedCallback));

        private static void OnViewKeyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var key = (LogGuardViewKeyDefinition)e.NewValue;
                if (Current.ViewMap.ContainsKey(key))
                {
                    Current.ViewMap.Remove(key);
                }
                Current.ViewMap.Add(key, d);
            }
            catch
            {

            }

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
                return LogGuardModuleManager.LGVH_Instance;
            }
        }

        public LogGuardViewHelper()
        {
            ViewMap = new Dictionary<LogGuardViewKeyDefinition, object>();
        }

        public void OnModuleStart()
        {
        }

        public object GetViewByKey(LogGuardViewKeyDefinition key)
        {
            return ViewMap[key];
        }
    }

}
