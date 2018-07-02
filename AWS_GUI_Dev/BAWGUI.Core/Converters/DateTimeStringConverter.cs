using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BAWGUI.Core.Converters
{
    public class DateTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == DependencyProperty.UnsetValue || String.IsNullOrEmpty((string)value))
            {
                return value;
            }
            else
            {
                switch ((string) parameter)
                {
                    case "DateOnly":
                        return value.ToString().Split(null)[0];
                    case "TimeOnly":
                        return value.ToString().Split(null)[1];
                    default:
                        return value;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return null;
            return DependencyProperty.UnsetValue;
        }
    }
}
