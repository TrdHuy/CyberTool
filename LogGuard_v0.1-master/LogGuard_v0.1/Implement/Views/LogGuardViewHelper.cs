using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace LogGuard_v0._1.Implement.Views
{
    public class LogGuardViewHelper
    {
        private static Dictionary<LogGuardViewKeyDefinition, object> ViewMap = new Dictionary<LogGuardViewKeyDefinition, object>();
        private static LogGuardViewHelper _instance;

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
                if (_instance == null)
                {
                    _instance = new LogGuardViewHelper();
                }
                return _instance;
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
