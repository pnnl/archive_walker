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
            //datetime object do not have microseconds resolution, so the number of ticks has to be manually calculated and added to the datetime object
            //there are 10 ticks per microsecond            
            System.DateTime dtDateTime = new DateTime(0001, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            double milisec = item * 1000.0;
            double diff = (milisec - 367000.0) * 86400.0;
            System.DateTime bbb = dtDateTime.AddMilliseconds(diff);
            double remainigTicks = (diff - Math.Floor(diff)) * 10000; //there are 10000 ticks per millisecond
            long rt = (long)Math.Round(remainigTicks);
            System.DateTime ccc = bbb.AddTicks(rt);
            return bbb;
        }

        public static double MatlabDateNumToDotNetSeconds(double item)
        {
            return (item - 367) * 86400;
        }

        public static string SecondsToDateTimeString(double item)
        {
            System.DateTime dtDateTime = new DateTime(0001, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            var bbb = dtDateTime.AddSeconds(item);
            return bbb.ToString("yyyyMMdd_HHmmss");
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
                                                                        "Green",
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

        public static IEnumerable<T> GetRow<T>(T[,] array, int index)
        {
            for (int i = 0; i < array.GetLength(1); i++)
            {
                yield return array[index, i];
            }
        }
    }
}
