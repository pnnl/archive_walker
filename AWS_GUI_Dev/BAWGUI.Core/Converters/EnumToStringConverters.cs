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
    public class EnumToStringConverter9 : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case DetectorWindowType.hann:
                    return "Hann";
                case DetectorWindowType.bartlett:
                    return "Bartlett";
                case DetectorWindowType.blackman:
                    return "Blackman";
                case DetectorWindowType.hamming:
                    return "Hamming";
                case DetectorWindowType.rectwin:
                    return "Rectangular";
                default:
                    throw new Exception("Detector window type not valid!");
            }
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "Hann":
                    return DetectorWindowType.hann;
                case "Bartlett":
                    return DetectorWindowType.bartlett;
                case "Blackman":
                    return DetectorWindowType.blackman;
                case "Hamming":
                    return DetectorWindowType.hamming;
                case "Rectangular":
                    return DetectorWindowType.rectwin;
                default:
                    throw new Exception("Enum type not valid!");
            }
        }
    }
}
