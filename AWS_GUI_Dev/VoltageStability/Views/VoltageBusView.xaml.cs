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

namespace VoltageStability.Views
{
    /// <summary>
    /// Interaction logic for VoltageBusView.xaml
    /// </summary>
    public partial class VoltageBusView : UserControl
    {
        public VoltageBusView()
        {
            InitializeComponent();
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var item = sender as TextBox;
            item.Background = Utility.HighlightColor;
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var item = sender as TextBox;
            item.Background = new SolidColorBrush(Colors.White);
        }
    }
}
