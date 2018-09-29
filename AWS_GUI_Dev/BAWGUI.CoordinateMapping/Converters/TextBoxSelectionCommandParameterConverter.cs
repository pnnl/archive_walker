using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace BAWGUI.CoordinateMapping.Converters
{
    public class TextBoxSelectionCommandParameterConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //return values.Clone();
            //if (Designer.IsInDesignMode) return false;

            var itemsControl = values[0] as ItemsControl;
            if (itemsControl == null)
            {
                return null;
            }
            var item = values[1];
            var itemContainer = itemsControl.ItemContainerGenerator.ContainerFromItem(item);

            // It may not yet be in the collection...
            if (itemContainer == null)
            {
                return Binding.DoNothing;
            }

            var itemIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer);
            var returnValues = new object[2];
            returnValues[1] = itemIndex;
            returnValues[0] = itemsControl.DataContext;
            return returnValues;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
