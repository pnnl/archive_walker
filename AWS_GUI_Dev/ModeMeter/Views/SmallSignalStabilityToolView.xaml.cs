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
            // try to find parent group box and highlight it.
            var c = VisualTreeHelper.GetChild(b,0);
        }
        private void FilterListBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as Expander;
            //b.Background = new SolidColorBrush(Colors.White);
            //var c = b.Content as Grid;
            //foreach (var ch in c.Children)
            //{
            //    if (ch is TextBlock)
            //    {
            //        var child = ch as TextBlock;
            //        child.Foreground = new SolidColorBrush(Colors.Black);
            //    }
            //}

        }

        private void FilterListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var b = sender as FilterListBox;
            var c = b.Content as Grid;
            foreach (var ch in c.Children)
            {
                if (ch is ListView)
                {
                    var child = ch as ListView;
                    child.Visibility = Visibility.Visible;
                    child.Focus();
                }
                if (ch is TextBlock)
                {
                    var child = ch as TextBlock;
                    child.Foreground = new SolidColorBrush(Colors.Red);
                    //child.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
