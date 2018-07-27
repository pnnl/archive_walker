using OxyPlot;
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
    /// Interaction logic for WindRampResultsView.xaml
    /// </summary>
    public partial class WindRampResultsView : UserControl
    {
        public WindRampResultsView()
        {
            InitializeComponent();
            //EventPlot.Controller = new OxyPlot.PlotController();
            //EventPlot.Controller.BindMouseDown(OxyPlot.OxyMouseButton.Left, OxyPlot.PlotCommands.ZoomRectangle);
            //EventPlot.Controller.BindMouseDown(OxyMouseButton.Right, PlotCommands.PanAt);
            ////RDPlot.Controller.BindMouseEnter(PlotCommands.HoverSnapTrack);
            //EventPlot.Controller.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PointsOnlyTrack);
        }
    }
}
