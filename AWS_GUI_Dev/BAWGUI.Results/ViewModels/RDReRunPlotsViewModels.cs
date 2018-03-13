using BAWGUI.RunMATLAB.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BAWGUI.Results.ViewModels
{
    public class RDreRunPlot : ViewModelBase
    {
        private ViewResolvingPlotModel _rdReRunPlotModel = new ViewResolvingPlotModel();
        public ViewResolvingPlotModel RDreRunPlotModel
        {
            get { return _rdReRunPlotModel; }
            set
            {
                _rdReRunPlotModel = value;
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
        private ObservableCollection<PlotModelThumbnailPair> _thumbnailPlots = new ObservableCollection<PlotModelThumbnailPair>();
        public ObservableCollection<PlotModelThumbnailPair> ThumbnailPlots
        {
            get { return _thumbnailPlots; }
            set
            {
                _thumbnailPlots = value;
                OnPropertyChanged();
            }
        }
        //private ObservableCollection<ViewResolvingPlotModel> _rdSignalPlotModels = new ObservableCollection<ViewResolvingPlotModel>();
        //public ObservableCollection<ViewResolvingPlotModel> RDSignalPlotModels
        //{
        //    get { return _rdSignalPlotModels; }
        //    set
        //    {
        //        _rdSignalPlotModels = value;
        //        OnPropertyChanged();
        //    }
        //}

        private PlotModelThumbnailPair _selectedSignalPlotModel = new PlotModelThumbnailPair();
        public PlotModelThumbnailPair SelectedSignalPlotModel
        {
            get { return _selectedSignalPlotModel; }
            set
            {
                _selectedSignalPlotModel = value;
                OnPropertyChanged();
            }
        }
    }
    public class PlotModelThumbnailPair:ViewModelBase
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
        private DataThresholdRMSPlotModelPair _signalPlotModelPair;
        public DataThresholdRMSPlotModelPair SignalPlotModelPair
        {
            get { return _signalPlotModelPair; }
            set
            {
                _signalPlotModelPair = value;
                OnPropertyChanged();
            }
        }
    }    

    public class DataThresholdRMSPlotModelPair:ViewModelBase
    {
        private ViewResolvingPlotModel _rdSignalPlotModel;
        public ViewResolvingPlotModel RDSignalPlotModel
        {
            get { return _rdSignalPlotModel; }
            set
            {
                _rdSignalPlotModel = value;
                OnPropertyChanged();
            }
        }
        private ViewResolvingPlotModel _rdThresholdRMSPlotModel;
        public ViewResolvingPlotModel RdThresholdRMSPlotModel
        {
            get { return _rdThresholdRMSPlotModel; }
            set
            {
                _rdThresholdRMSPlotModel = value;
                OnPropertyChanged();
            }
        }
    }
}
