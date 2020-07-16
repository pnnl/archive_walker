using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;
using System.Drawing;
using System.Collections;

namespace BAWGUI.Utilities
{
    public class EnumerationManager
    {
        public static Array GetValues(Type enumeration)
        {
            Array wArray = Enum.GetValues(enumeration);
            ArrayList wFinalArray = new ArrayList();
            foreach (Enum wValue in wArray)
            {
                FieldInfo fi = enumeration.GetField(wValue.ToString());
                if (fi != null)
                {
                    BrowsableAttribute[] wBrowsableAttributes = fi.GetCustomAttributes(typeof(BrowsableAttribute), true) as BrowsableAttribute[];
                    if (wBrowsableAttributes.Length > 0)
                    {
                        // If the Browsable attribute is false
                        if (wBrowsableAttributes[0].Browsable == false)
                            // Do not add the enumeration to the list.
                            continue;
                    }

                    DescriptionAttribute[] wDescriptions = fi.GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
                    if (wDescriptions.Length > 0)
                        wFinalArray.Add(wDescriptions[0].Description);
                    else
                        wFinalArray.Add(wValue.ToString());
                }
            }
            return wFinalArray.ToArray();
        }
    }
}
