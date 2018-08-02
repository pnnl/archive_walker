using BAWGUI.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BAWGUI.Core.Converters
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
}
