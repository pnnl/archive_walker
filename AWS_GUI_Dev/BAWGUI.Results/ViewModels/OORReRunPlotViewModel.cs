using BAWGUI.Core;
using BAWGUI.RunMATLAB.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace BAWGUI.Results.ViewModels
{
    public class OORReRunPlot: Core.ViewModelBase
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
        private ObservableCollection<PlotModelThumbnailOORTriple> _thumbnailPlots = new ObservableCollection<PlotModelThumbnailOORTriple>();
        public ObservableCollection<PlotModelThumbnailOORTriple> ThumbnailPlots
        {
            get { return _thumbnailPlots; }
            set
            {
                _thumbnailPlots = value;
                OnPropertyChanged();
            }
        }

        private ViewResolvingPlotModel _oorReRunAllSignalsPlotModel;
        public ViewResolvingPlotModel OORReRunAllSignalsPlotModel
        {
            get { return _oorReRunAllSignalsPlotModel; }
            set
            {
                _oorReRunAllSignalsPlotModel = value;
                OnPropertyChanged();
            }
        }
        private PlotModelThumbnailOORTriple _selectedSignalPlotModel = new PlotModelThumbnailOORTriple();
        public PlotModelThumbnailOORTriple SelectedSignalPlotModel
        {
            get { return _selectedSignalPlotModel; }
            set
            {
                _selectedSignalPlotModel = value;
                OnPropertyChanged();
            }
        }
        private bool _isByDuration = true;
        public bool IsByDuration
        {
            get { return _isByDuration; }
            set
            {
                _isByDuration = value;
                OnPropertyChanged();
            }
        }
        private bool _isByROC = true;
        public bool IsByROC
        {
            get { return _isByROC; }
            set
            {
                _isByROC = value;
                OnPropertyChanged();
            }
        }
    }
    public class PlotModelThumbnailOORTriple : ViewModelBase
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
        /// <summary>
        /// Holds the name of the signal
        /// </summary>
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
        private OORSignalDurationROCPlotModelTriple _signalPlotModelTriple;
        public OORSignalDurationROCPlotModelTriple SignalPlotModelTriple
        {
            get { return _signalPlotModelTriple; }
            set
            {
                _signalPlotModelTriple = value;
                OnPropertyChanged();
            }
        }
        private bool _isByDuration = true;
        public bool IsByDuration
        {
            get { return _isByDuration; }
            set
            {
                _isByDuration = value;
                OnPropertyChanged();
            }
        }
        private bool _isByROC = true;
        public bool IsByROC
        {
            get { return _isByROC; }
            set
            {
                _isByROC = value;
                OnPropertyChanged();
            }
        }
    }
    public class OORSignalDurationROCPlotModelTriple : ViewModelBase
    {
        private ViewResolvingPlotModel _oorSignalPlotModel = new ViewResolvingPlotModel();
        public ViewResolvingPlotModel OORSignalPlotModel
        {
            get { return _oorSignalPlotModel; }
            set
            {
                _oorSignalPlotModel = value;
                OnPropertyChanged();
            }
        }
        private ViewResolvingPlotModel _oorDurationPlotModel = new ViewResolvingPlotModel();
        public ViewResolvingPlotModel OORDurationPlotModel
        {
            get { return _oorDurationPlotModel; }
            set
            {
                _oorDurationPlotModel = value;
                OnPropertyChanged();
            }
        }
        private ViewResolvingPlotModel _oorROCPlotModel = new ViewResolvingPlotModel();
        public ViewResolvingPlotModel OORROCPlotModel
        {
            get { return _oorROCPlotModel; }
            set
            {
                _oorROCPlotModel = value;
                OnPropertyChanged();
            }
        }
    }
}