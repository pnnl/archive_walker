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

namespace BAWGUI.Results.Views
{
    /// <summary>
    /// Interaction logic for OccurrenceTableView.xaml
    /// </summary>
    public partial class OccurrenceTableView : UserControl
    {
        public OccurrenceTableView()
        {
            InitializeComponent();
        }

        private void DataGridControl_SelectionChanged(object sender, Xceed.Wpf.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            var s = (Xceed.Wpf.DataGrid.DataGridControl)sender;
            if (s.SelectedItem != null)
            {
                s.CurrentItem = s.SelectedItem;
            }
            //Xceed.Wpf.DataGrid.DataRow row = s.GetContainerFromItem(s.CurrentItem) as Xceed.Wpf.DataGrid.DataRow;
            //if (row != null)
            //{
            //    row.InactiveSelectionBackground = Brushes.Green;
            //    //row.SelectionForeground = New Media.SolidColorBrush(Colors.White);
            //}
        }
    }
}
