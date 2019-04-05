using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BAWGUI.Core.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var a = (OxyPlot.OxyColor)value;
            var b = new SolidColorBrush(Color.FromArgb(a.A, a.R, a.G, a.B));
            return b;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    public class OxyColorToGradientBrushConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colors = (List<OxyPlot.OxyColor>)value;
            colors = colors.AsEnumerable().Reverse().ToList();
            //colors.Reverse();
            var brush = new LinearGradientBrush()
            {
                StartPoint = new Point(),
                EndPoint = new Point(0, 1)
            };
            if (colors.Count != 0)
            {
                var w = 1d / colors.Count;
                if (colors.Count > 1)
                {
                    for (var i = 0; i < colors.Count - 1; i++)
                    {
                        var offset = w * (i + 1);
                        var c1 = Color.FromArgb(colors[i].A, colors[i].R, colors[i].G, colors[i].B);
                        var c2 = Color.FromArgb(colors[i + 1].A, colors[i + 1].R, colors[i + 1].G, colors[i + 1].B);
                        var stop1 = new GradientStop(c1, offset);
                        var stop2 = new GradientStop(c2, offset);
                        brush.GradientStops.Add(stop1);
                        brush.GradientStops.Add(stop2);
                    }
                }
                else
                {
                    var c1 = Color.FromArgb(colors[0].A, colors[0].R, colors[0].G, colors[0].B);
                    var stop1 = new GradientStop(c1, 0);
                    brush.GradientStops.Add(stop1);
                    var stop2 = new GradientStop(c1, 1);
                    brush.GradientStops.Add(stop2);
                }
            }
            //var c = Color.FromArgb(colors[colors.Count-1].A, colors[colors.Count - 1].R, colors[colors.Count - 1].G, colors[colors.Count - 1].B);
            //var stop = new GradientStop(c, 1);
            //brush.GradientStops.Add(stop);
            return brush;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
