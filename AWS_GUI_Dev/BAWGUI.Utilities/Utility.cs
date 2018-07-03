using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BAWGUI.Utilities
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

        public static string FindFirstInputFile(string filename, string fileType)
        {
            var firstFile = "";
            if (File.Exists(filename))
            {
                if (Path.GetExtension(filename).Substring(1).ToLower() == fileType)
                    firstFile = filename;
                //return firstFile;
            }
            else if (Directory.Exists(filename))
            {
                foreach (var file in Directory.GetFiles(filename))
                {
                    firstFile = FindFirstInputFile(file, fileType);
                    if (!string.IsNullOrEmpty(firstFile))
                    {
                        break;
                    }
                }
                foreach (var path in Directory.GetDirectories(filename))
                {
                    if (!string.IsNullOrEmpty(firstFile))
                    {
                        break;
                    }
                    firstFile = FindFirstInputFile(path, fileType);
                }
            }
            else
                throw new Exception("\nError: data file path \"" + filename + "\" does not exists!");
            return firstFile;
        }
    }
}
