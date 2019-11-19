using BAWGUI.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BAWGUI.CoordinateMapping.Converters
{
    //public class SiteCountVisibilityConverter : IValueConverter
    //{
    //    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if ((int)value > 2)
    //        {
    //            return Visibility.Visible;
    //        }
    //        else
    //        {
    //            return Visibility.Collapsed;
    //        }
    //    }

    //    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return DependencyProperty.UnsetValue;
    //    }
    //}
    public class SiteCountVisibilityConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue)
            {
                SignalMapPlotType type = (SignalMapPlotType)values[0];
                int count = (int)values[1];
                if (type == SignalMapPlotType.Area)
                {
                    if (count > 3)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else
                {
                    if (count > 2)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
