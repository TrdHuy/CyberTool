using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace honeyboard_release_service.views.elements.commit_data_grid.converter
{
    internal class GridLineToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is DataGridGridLinesVisibility
                && parameter is DataGridGridLinesVisibility)
            {
                flag = (DataGridGridLinesVisibility)value == (DataGridGridLinesVisibility)parameter;
            }
            return flag;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    return (DataGridGridLinesVisibility)parameter;
                }
            }
            return DataGridGridLinesVisibility.None;
        }
    }
}
