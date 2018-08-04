using BAWGUI.Core.Models;
using ModeMeter.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ModeMeter.Converters
{
    public class RetroactiveContinuityStatusVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case RetroactiveContinuityStatusType.OFF:
                    return Visibility.Collapsed;
                case RetroactiveContinuityStatusType.ON:
                    return Visibility.Visible;
                default:
                    throw new Exception("Retroactive Continuity Status Type not valid!");
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
