using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VoltageStability.ViewModels
{
    public class VSReRunPlot : ViewModelBase
    {
        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<PlotModelThumbnailVSPair> _thumbnailPlots = new ObservableCollection<PlotModelThumbnailVSPair>();
        public ObservableCollection<PlotModelThumbnailVSPair> ThumbnailPlots
        {
            get { return _thumbnailPlots; }
            set
            {
                _thumbnailPlots = value;
                OnPropertyChanged();
            }
        }
        private PlotModelThumbnailVSPair _selectedSignalPlotModel = new PlotModelThumbnailVSPair();
        public PlotModelThumbnailVSPair SelectedSignalPlotModel
        {
            get { return _selectedSignalPlotModel; }
            set
            {
                _selectedSignalPlotModel = value;
                OnPropertyChanged();
            }
        }

    }

    public class PlotModelThumbnailVSPair : ViewModelBase
    {
        private BitmapSource _thumbnail;
        public BitmapSource Thumbnail
        {
            get { return _thumbnail; }
            set
            {
                _thumbnail = value;
                OnPropertyChanged();
            }
        }
        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                OnPropertyChanged();
            }
        }
        private ViewResolvingPlotModel _vsSignalPlotModel = new ViewResolvingPlotModel();
        public ViewResolvingPlotModel VSSignalPlotModel
        {
            get { return _vsSignalPlotModel; }
            set
            {
                _vsSignalPlotModel = value;
                OnPropertyChanged();
            }
        }
    }
}
