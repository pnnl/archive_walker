using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Resources.Utility
{
    public class Utility
    {
        public static DateTime MatlabDateNumToDotNetDateTime(double item)
        {
            System.DateTime dtDateTime = new DateTime(0001, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            System.DateTime bbb = dtDateTime.AddSeconds((item - 367) * 86400);
            return bbb;
        }

        //public static Color HighlightColor = Colors.Cornsilk;
        public static System.Windows.Media.SolidColorBrush HighlightColor = new System.Windows.Media.SolidColorBrush(Colors.Cornsilk);
    }
}
