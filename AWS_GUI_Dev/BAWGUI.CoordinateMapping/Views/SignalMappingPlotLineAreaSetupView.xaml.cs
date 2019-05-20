using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/// <summary>
/// Define each signal's site choice when the type is line or area
/// </summary>
namespace BAWGUI.CoordinateMapping.Views
{
    /// <summary>
    /// Interaction logic for SignalMappingPlotLineSetupView.xaml
    /// </summary>
    public partial class SignalMappingPlotLineAreaSetupView : UserControl
    {
        public SignalMappingPlotLineAreaSetupView()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var txtbx = sender as TextBox;
            txtbx.Background = Utility.HighlightColor;
            //var parent = VisualTreeHelper.GetParent(txtbx);
            //var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            //for (int index = 0; index < childrenCount; index++)
            //{
            //    var child = VisualTreeHelper.GetChild(parent, index);
            //    if (child is TextBox)
            //    {
            //        var chld = child as TextBox;
            //        chld.Background = Utility.HighlightColor;
            //    }
            //}
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtbx = sender as TextBox;
            txtbx.Background = new SolidColorBrush(Colors.White);
            //var parent = VisualTreeHelper.GetParent(txtbx);
            //var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            //for (int index = 0; index < childrenCount; index++)
            //{
            //    var child = VisualTreeHelper.GetChild(parent, index);
            //    if (child is TextBox)
            //    {
            //        var chld = child as TextBox;
            //        chld.Background = new SolidColorBrush(Colors.White);
            //    }
            //}
        }
    }
}
