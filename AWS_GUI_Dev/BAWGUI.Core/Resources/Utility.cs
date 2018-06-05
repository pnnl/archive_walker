using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BAWGUI.Core.Resources
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


        public static BitmapSource ResizeBitmapSource(BitmapSource bitmapSource, double v1)
        {
            var w = bitmapSource.PixelWidth;
            //var h = bitmapSource.PixelHeight;
            var scale = v1 / w;
            WriteableBitmap writable = new WriteableBitmap(new TransformedBitmap(bitmapSource, new ScaleTransform(scale, scale)));
            writable.Freeze();
            return writable;
        }

        public static List<string> SaturatedColors = new List<string>{  "Blue",
                                                                        "Orange",
                                                                        "Black",
                                                                        "Purple",
                                                                        "Cyan",
                                                                        "Maroon",
                                                                        "Brown",
                                                                        "Magenta",
                                                                        "Lime",
                                                                        "Pink",
                                                                        "Teal",
                                                                        "Coral",
                                                                        "Olive",
                                                                        "Turquoise",
                                                                        "Mint",
                                                                        "Red",
                                                                        "Green",
                                                                        "Yellow",
                                                                        "RoyalBlue",
                                                                        "LimeGreen"
        };
    }
}
