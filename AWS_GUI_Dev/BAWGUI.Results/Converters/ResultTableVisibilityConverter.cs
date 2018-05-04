using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BAWGUI.Results.Converters
{
    public class ResultTableVisibilityConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var par = parameter.ToString();
            if (values[1] != System.Windows.DependencyProperty.UnsetValue && (bool)values[1])
            {
                if (par == "NoEvent" || par == "Table")
                {
                    if ((values[0] != System.Windows.DependencyProperty.UnsetValue && (int)values[0] == 0) ^ par == "NoEvent")
                    {
                        return System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        return System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    return System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                if (par == "NoResult")
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Collapsed;
                }
            }

            //if (par == "NoEvent" || par == "Table")
            //{
            //    if ((values[0] != System.Windows.DependencyProperty.UnsetValue && (int)values[0] == 0) ^ par == "NoEvent")
            //    {
            //        return System.Windows.Visibility.Hidden;
            //    }
            //    else
            //    {
            //        return System.Windows.Visibility.Visible;
            //    }
            //}
            //else
            //{
            //    if (values[1] != System.Windows.DependencyProperty.UnsetValue && !(bool)values[1])
            //    {
            //        return System.Windows.Visibility.Visible;
            //    }
            //    else
            //    {
            //        return System.Windows.Visibility.Hidden;
            //    }
            //}


            //if (values[0] != System.Windows.DependencyProperty.UnsetValue && )
            //{
            //    if (values[1] != System.Windows.DependencyProperty.UnsetValue && (bool)values[1])
            //    {
            //        return System.Windows.Visibility.Visible;
            //    }
            //    else
            //    {
            //        if ((int)values[0] > 0)
            //        {
            //            return System.Windows.Visibility.Visible;
            //        }
            //        else
            //        {
            //            return System.Windows.Visibility.Visible;
            //        }
            //    }
            //}
            //else
            //{
            //    return System.Windows.Visibility.Visible;
            //}
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
