using BAWGUI.ViewModels;
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

namespace BAWGUI.Views
{
    /// <summary>
    /// Interaction logic for ProjectTreeView.xaml
    /// </summary>
    public partial class ProjectTreeView : UserControl
    {
        public ProjectTreeView()
        {
            InitializeComponent();
        }

        private void myProjectTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void myProjectTreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
            //treeViewItem.DataContext.IsSelected = true;
            var st = sender as TreeViewItem;
            var selectedProject = st.DataContext as AWProjectViewModel;
            if (selectedProject.SelectedRun != null)
            {
                selectedProject.SelectedRun.IsSelected = false;
            }
            var selectedRun = treeViewItem.DataContext as RunMATLAB.ViewModels.AWRunViewModel;
            if (selectedRun != null)
            {
                selectedRun.IsSelected = true;
            }
        }
        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }
    }
}
