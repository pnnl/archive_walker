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

namespace ModeMeter.Views
{
    /// <summary>
    /// Interaction logic for ModeSignalSelectionView.xaml
    /// </summary>
    public partial class ModeSignalSelectionView : UserControl
    {
        public ModeSignalSelectionView()
        {
            InitializeComponent();
        }

        private void FilterListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as FilterListBox;
            b.Background = Utility.HighlightColor;
            //foreach (var item in b.Parent)
            //{

            //}
        }

        private void FilterListBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var b = sender as FilterListBox;
            b.Background = new SolidColorBrush(Colors.White);

        }
    }
}
