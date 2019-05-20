using BAWGUI.Core.Views;
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

namespace ModeMeter.Views
{
    /// <summary>
    /// Interaction logic for SmallSignalStabilityToolView.xaml
    /// </summary>
    public partial class SmallSignalStabilityToolView : UserControl
    {
        public SmallSignalStabilityToolView()
        {
            InitializeComponent();
        }

        private void FilterListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as FilterListBox;
            b.Background = Utility.HighlightColor;
            var c = VisualTreeHelper.GetChild(b,0);
        }
        private void FilterListBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as FilterListBox;
            b.Background = new SolidColorBrush(Colors.White);

        }
    }
}
