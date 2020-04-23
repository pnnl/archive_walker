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
    public class EnumToStringConverter20 : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case ForgetFactor1Type.FALSE:
                    return "Disable";
                case ForgetFactor1Type.TRUE:
                    return "Enable";
                default:
                    throw new Exception("Forget Factor 1 type not valid!");
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "Disable":
                    return ForgetFactor1Type.FALSE;
                case "Enable":
                    return ForgetFactor1Type.TRUE;
                default:
                    throw new Exception("Enum type not valid!");
            }
        }
    }
    public class EnumToStringConverter21 : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case ForgetFactor2Type.FALSE:
                    return "Disable";
                case ForgetFactor2Type.TRUE:
                    return "Match Smallest Analysis Window";
                case ForgetFactor2Type.MATCH:
                    return "Match Full Analysis Window";
                default:
                    throw new Exception("Forget Factor 2 type not valid!");
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "Disable":
                    return ForgetFactor2Type.FALSE;
                case "Match Smallest Analysis Window":
                    return ForgetFactor2Type.TRUE;
                case "Match Full Analysis Window":
                    return ForgetFactor2Type.MATCH;
                default:
                    throw new Exception("Enum type not valid!");
            }
        }
    }
    public class EnumToStringConverter22 : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case PostEventWinAdjType.NONE:
                    return "None";
                case PostEventWinAdjType.SHORTEN:
                    return "Shorten";
                case PostEventWinAdjType.DIMINISH:
                    return "Diminish";
                default:
                    throw new Exception("Post Event Win Adj type not valid!");
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "None":
                    return PostEventWinAdjType.NONE;   
                case "Shorten":
                    return PostEventWinAdjType.SHORTEN;
                case "Diminish":
                    return PostEventWinAdjType.DIMINISH;
                default:
                    throw new Exception("Enum type not valid!");
            }
        }
    }
}
