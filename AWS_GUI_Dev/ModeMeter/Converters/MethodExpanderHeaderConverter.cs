using ModeMeter.Models;
using ModeMeter.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModeMeter.Converters
{
    public class MethodExpanderHeaderConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null)
            {
                var firstString = "";
                var secondString = "";
                switch (values[2])
                {
                    case ModeMethods.YWARMA:
                        secondString = "YW-ARMA";
                        break;
                    case ModeMethods.LSARMA:
                        secondString = "LS-ARMA";
                        break;
                    case ModeMethods.YWARMAS:
                        secondString = "YW-ARMA+S";
                        break;
                    case ModeMethods.LSARMAS:
                        secondString = "LS-ARMA+S";
                        break;
                    case ModeMethods.STLS:
                        secondString = "STLS";
                        break;
                    case ModeMethods.STLSS:
                        secondString = "STLS+S";
                        break;
                    default:
                        throw new Exception("Mode method type not valid!");
                }
                if (values[1].GetType() == typeof(ObservableCollection<ModeMethodViewModel>))
                {
                    var methodList = (ObservableCollection<ModeMethodViewModel>)values[1];
                    var thisDetector = (ModeMethodViewModel)values[0];
                    firstString = (methodList.IndexOf(thisDetector) + 1).ToString();
                    //secondString = thisDetector.Name.ToString();
                }
                return "Method " + firstString + ": " + secondString;
            }
            else
                return "No detector Selected Yet!";

        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
