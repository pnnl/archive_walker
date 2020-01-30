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

namespace BAWGUI.CoordinateMapping.Views
{
    /// <summary>
    /// Interaction logic for DEFAreaSitePlotDotSetupView.xaml
    /// </summary>
    public partial class DEFAreaSitePlotDotSetupView : UserControl
    {
        public DEFAreaSitePlotDotSetupView()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var txtbx = sender as TextBox;
            txtbx.Background = Utility.HighlightColor;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtbx = sender as TextBox;
            txtbx.Background = new SolidColorBrush(Colors.White);
        }
    }
}
