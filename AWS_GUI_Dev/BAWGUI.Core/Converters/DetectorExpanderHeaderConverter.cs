using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Microsoft.VisualBasic;
namespace BAWGUI.Core.Converters
{
    public class DetectorExpanderHeaderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null)
            {
                var firstString = "";
                var secondString = "";
                if (values[1].GetType() == typeof(ObservableCollection<AlarmingDetectorBase>))
                {
                    var detectorList = (ObservableCollection<AlarmingDetectorBase>)values[1];
                    var thisDetector = (AlarmingDetectorBase)values[0];
                    firstString = (detectorList.IndexOf(thisDetector) + 1).ToString();
                    secondString = thisDetector.Name;
                }
                if (values[1].GetType() == typeof(ObservableCollection<DetectorBase>))
                {
                    var detectorList = (ObservableCollection<DetectorBase>)values[1];
                    var thisDetector = (DetectorBase)values[0];
                    firstString = (detectorList.IndexOf(thisDetector) + 1).ToString();
                    secondString = thisDetector.Name;
                }
                return "Detector " + firstString + " - " + secondString;
            }
            else
                return "No detector Selected Yet!";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
