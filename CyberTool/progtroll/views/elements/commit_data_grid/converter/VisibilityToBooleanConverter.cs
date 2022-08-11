using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace progtroll.views.elements.commit_data_grid.converter
{
    internal class VisibilityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is Visibility)
            {
                flag = (Visibility)value == Visibility.Visible;
            }
            return flag;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility flag = Visibility.Collapsed;
            if (value is bool && (bool)value)
            {
                flag = Visibility.Visible;
            }
            return flag;
        }
    }
}
