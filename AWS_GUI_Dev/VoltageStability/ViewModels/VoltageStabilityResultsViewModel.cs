using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoltageStability.Models;

namespace VoltageStability.ViewModels
{
    public class VoltageStabilityResultsViewModel : ViewModelBase
    {
        public VoltageStabilityResultsViewModel()
        {
            //_configFilePath = "";
            //_run = new AWRunViewModel();
            _results = new ObservableCollection<VoltageStabilityEventViewModel>();
            _filteredResults = new ObservableCollection<VoltageStabilityEventViewModel>();
            _models = new List<VoltageStabilityEvent>();
            //_engine = RunMATLAB.ViewModels.MatLabEngine.Instance;
            //RunSparseMode = new RelayCommand(_runSparseMode);
            //OutOfRangeReRun = new RelayCommand(_outOfRangeRerun);
            //CancelOutOfRangeReRun = new RelayCommand(_cancelOORReRun);
            //_sparsePlotModels = new ObservableCollection<SparsePlot>();
            //_oorReRunPlotModels = new ObservableCollection<OORReRunPlot>();
        }

        //private AWRunViewModel _run;
        //public AWRunViewModel Run
        //{
        //    get { return _run; }
        //    set
        //    {
        //        _run = value;
        //        if (System.IO.File.Exists(_run.Model.ConfigFilePath))
        //        {
        //            ConfigFilePath = _run.Model.ConfigFilePath;
        //        }
        //        OnPropertyChanged();
        //    }
        //}

        private List<VoltageStabilityEvent> _models;
        public List<VoltageStabilityEvent> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                _results.Clear();
                _filteredResults.Clear();
                foreach (var model in value)
                {
                    _results.Add(new VoltageStabilityEventViewModel(model));
                    _filteredResults.Add(new VoltageStabilityEventViewModel(model));
                }
                OnPropertyChanged();
            }
        }
        //private RunMATLAB.ViewModels.MatLabEngine _engine;
        //public RunMATLAB.ViewModels.MatLabEngine Engine
        //{
        //    get { return _engine; }
        //}
        private ObservableCollection<VoltageStabilityEventViewModel> _results;
        private ObservableCollection<VoltageStabilityEventViewModel> _filteredResults;
        public ObservableCollection<VoltageStabilityEventViewModel> FilteredResults
        {
            get { return _filteredResults; }
            set
            {
                _filteredResults = value;
                OnPropertyChanged();
            }
        }

        private string _selectedStartTime;
        public string SelectedStartTime
        {
            get { return _selectedStartTime; }
            set
            {
                _selectedStartTime = value;
                OnPropertyChanged();
                var startTime = Convert.ToDateTime(value);
                var endTime = Convert.ToDateTime(_selectedEndTime);
                if (startTime <= endTime)
                {
                    _filterTableByTime();
                }
                else
                {
                    //MessageBox.Show("Selected start time is later than end time.", "Error!", MessageBoxButtons.OK);
                }
            }
        }

        private string _selectedEndTime;
        public string SelectedEndTime
        {
            get { return _selectedEndTime; }
            set
            {
                _selectedEndTime = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(_selectedStartTime))
                {
                    var startTime = Convert.ToDateTime(_selectedStartTime);
                    var endTime = Convert.ToDateTime(value);
                    if (startTime <= endTime)
                    {
                        _filterTableByTime();
                    }
                    else
                    {
                        //MessageBox.Show("Selected end time is earlier than start time.", "Error!", MessageBoxButtons.OK);
                    }
                }
            }
        }
        private void _filterTableByTime()
        {
            ObservableCollection<VoltageStabilityEventViewModel> newResults = new ObservableCollection<VoltageStabilityEventViewModel>();
            DateTime startT = DateTime.Parse(_selectedStartTime);
            DateTime endT = DateTime.Parse(_selectedEndTime);
            foreach (var evnt in _results)
            {
                DateTime st = DateTime.Parse(evnt.StartTime);
                DateTime ed = DateTime.Parse(evnt.EndTime);
                if (DateTime.Compare(st, endT) <= 0 && DateTime.Compare(ed, startT) >= 0)
                {
                    newResults.Add(evnt);
                }
            }
            FilteredResults = new ObservableCollection<VoltageStabilityEventViewModel>(newResults.OrderBy(x => x.StartTime));
        }
        private VoltageStabilityEventViewModel _selectedVSEvent;
        public VoltageStabilityEventViewModel SelectedVSEvent
        {
            get { return _selectedVSEvent; }
            set
            {
                _selectedVSEvent = value;
                if (_selectedVSEvent != null)
                {
                    //foreach (var plotM in SparsePlotModels)
                    //{
                    //    var lowerRange = DateTime.ParseExact(_selectedVSEvent.StartTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                    //    var higherRange = DateTime.ParseExact(_selectedVSEvent.EndTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                    //    double axisMin = 0d, axisMax = 0d;
                    //    foreach (var axis in plotM.SparsePlotModel.Axes)
                    //    {
                    //        if (axis.IsVertical())
                    //        {
                    //            axisMin = axis.Minimum;
                    //            axisMax = axis.Maximum;
                    //        }
                    //    }
                    //    var rectAnnotation = new OxyPlot.Annotations.RectangleAnnotation()
                    //    {
                    //        Fill = OxyColor.FromArgb(75, 255, 0, 0),
                    //        //MinimumX = lowerRange,
                    //        //MaximumX = higherRange,
                    //        //Fill = OxyColors.Red,
                    //        MinimumX = lowerRange - (higherRange - lowerRange),
                    //        MaximumX = higherRange + (higherRange - lowerRange),
                    //        MinimumY = axisMin,
                    //        MaximumY = axisMax
                    //    };
                    //    plotM.SparsePlotModel.Annotations.Clear();
                    //    plotM.SparsePlotModel.Annotations.Add(rectAnnotation);
                    //    plotM.SparsePlotModel.InvalidatePlot(true);
                    //}
                }
                OnPropertyChanged();
            }
        }

    }
}
