using LogGuard_v0._1.LogGuard.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LogGuard_v0._1.LogGuard.Control
{
    public static class LogGuardStatic
    {
        private static ResourceDictionary LogGuardRes = (ResourceDictionary)Application.LoadComponent(new Uri("/LogGuard_v0.1;component/LogGuard/Resources/LogWatcher.xaml", UriKind.Relative));

        private static Style _logGuardColumnHeaderSytleCache;
        private static Brush _extrusionBorderBackgroundCache;
        private static Brush _scrollbarBackgroundCache;

        public static Style LogGuardColumnHeaderSytle
        {
            get
            {
                if(_logGuardColumnHeaderSytleCache == null)
                {
                    _logGuardColumnHeaderSytleCache = GetResource(typeof(GridViewColumnHeader)) as Style;
                }
                return _logGuardColumnHeaderSytleCache;
            }
        }

        public static Brush ExtrusionBorderBackgroundBrush
        {
            get
            {
                if (_extrusionBorderBackgroundCache == null)
                {
                    _extrusionBorderBackgroundCache = GetResource("ExtrusionBorderBackground") as Brush;
                }
                return _extrusionBorderBackgroundCache;
            }
        }

        public static Brush ScrollBarBackgroundBrush
        {
            get
            {
                if (_scrollbarBackgroundCache == null)
                {
                    _scrollbarBackgroundCache = GetResource("ScrollBarBackground") as Brush;
                }
                return _scrollbarBackgroundCache;
            }
        }

        private static object GetResource(object keyId)
        {
            var result = ((Style)LogGuardRes[typeof(LogWatcher)])?.Resources[keyId];
            return result;
        }

        public static ResourceKey GridViewWatcherHeaderHeightKey;
    }
}
