using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BAWGUI.Core.Converters
{
    public class SelectionStatusBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "White";
            else
                return "WhiteSmoke";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
