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
    public class EnumToStringConverter11 : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case OutputSignalStorageType.CreateCustomPMU:
                    return "Create Custom PMU";
                case OutputSignalStorageType.ReplaceInput:
                    return "Replace Input";
                default:
                    throw new Exception("Output signal storage type not valid!");
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "Create Custom PMU":
                    return OutputSignalStorageType.CreateCustomPMU;
                case "Replace Input":
                    return OutputSignalStorageType.ReplaceInput;
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
                case SignalMapPlotType.Dot:
                    return "Dot";
                case SignalMapPlotType.Line:
                    return "Line";
                case SignalMapPlotType.Area:
                    return "Area";
                default:
                    throw new Exception("Signal plot type on map not valid!");
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "Dot":
                    return SignalMapPlotType.Dot;
                case "Line":
                    return SignalMapPlotType.Line;
                case "Area":
                    return SignalMapPlotType.Area;
                default:
                    throw new Exception("Enum type not valid!");
            }
        }
    }
    public class EnumToStringConverter13 : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case RMSEnergyBandOptions.Band1:
                    return "Band 1";
                case RMSEnergyBandOptions.Band2:
                    return "Band 2";
                case RMSEnergyBandOptions.Band3:
                    return "Band 3";
                case RMSEnergyBandOptions.Band4:
                    return "Band 4";
                default:
                    throw new Exception("Band type not valid!");
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "Band 1":
                    return RMSEnergyBandOptions.Band1;
                case "Band 2":
                    return RMSEnergyBandOptions.Band2;
                case "Band 3":
                    return RMSEnergyBandOptions.Band3;
                case "Band 4":
                    return RMSEnergyBandOptions.Band4;
                default:
                    throw new Exception("Enum type not valid!");
            }
        }
    }
}
