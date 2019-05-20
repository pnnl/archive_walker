using ModeMeter.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModeMeter.Converters
{
    public class EnumToStringConverter11 : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case RetroactiveContinuityStatusType.OFF:
                    return "OFF";
                case RetroactiveContinuityStatusType.ON:
                    return "ON";
                default:
                    throw new Exception("Retroactive Continuity Status Type not valid!");
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "OFF":
                    return RetroactiveContinuityStatusType.OFF;

                case "ON":
                    return RetroactiveContinuityStatusType.ON;

                default:
                    throw new Exception("Enum type not valid!");
            }
        }
    }
    public class EnumToStringConverter12 : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case ModeMethods.YWARMA:
                    return "YW-ARMA";
                case ModeMethods.LSARMA:
                    return "LS-ARMA";
                case ModeMethods.YWARMAS:
                    return "YW-ARMA+S";
                case ModeMethods.LSARMAS:
                    return "LS-ARMA+S";
                default:
                    throw new Exception("Mode method type not valid!");
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "YW-ARMA":
                    return ModeMethods.YWARMA;
                case "LS-ARMA":
                    return ModeMethods.LSARMA;
                case "YW-ARMA+S":
                    return ModeMethods.YWARMAS;
                case "LS-ARMA+S":
                    return ModeMethods.LSARMAS;
                default:
                    throw new Exception("Enum type not valid!");
            }
        }
    }
}
