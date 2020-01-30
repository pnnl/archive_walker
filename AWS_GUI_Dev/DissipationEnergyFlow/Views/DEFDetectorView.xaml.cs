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

namespace DissipationEnergyFlow.Views
{
    /// <summary>
    /// Interaction logic for DEFDetectorView.xaml
    /// </summary>
    public partial class DEFDetectorView : UserControl
    {
        public DEFDetectorView()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as TextBox;
            var stkl = b.Parent as StackPanel;
            foreach (var item in stkl.Children)
            {
                if (item is TextBox)
                {
                    var ttbx = item as TextBox;
                    ttbx.Background = Utility.HighlightColor;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as TextBox;
            var stkl = b.Parent as StackPanel;
            foreach (var item in stkl.Children)
            {
                if (item is TextBox)
                {
                    var ttbx = item as TextBox;
                    ttbx.Background = new SolidColorBrush(Colors.White);
                }
            }
        }
    }
}
