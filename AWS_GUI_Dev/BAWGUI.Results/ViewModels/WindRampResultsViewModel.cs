using BAWGUI.Core;
using BAWGUI.MATLABRunResults.Models;
using BAWGUI.Results.Models;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace BAWGUI.Results.ViewModels
{
    public class WindRampResultsViewModel:ViewModelBase
    {
        public WindRampResultsViewModel()
        {
            _configFilePath = "";
            _run = new AWRunViewModel();
            _results = new System.Collections.ObjectModel.ObservableCollection<WindRampEventViewModel>();
            _filteredResults = new ObservableCollection<WindRampEventViewModel>();
            _models = new List<WindRampEvent>();
            _engine = RunMATLAB.ViewModels.MatLabEngine.Instance;
            RunSparseMode = new RelayCommand(_runSparseMode);
            _sparsePlotModels = new ObservableCollection<SparsePlot>();
            _wrSignals = new Dictionary<string, string>();
            SignalSelectionChanged = new RelayCommand(_sortSignalSelection);
            PlotWindRampEvent = new RelayCommand(_drawWREventPlots);
        }
        private string _configFilePath;
        public string ConfigFilePath
        {
            get { return _configFilePath; }
            set
            {
                _configFilePath = value;
                OnPropertyChanged();
            }
        }
        private AWRunViewModel _run;
        public AWRunViewModel Run
        {
            get { return _run; }
            set
            {
                _run = value;
                if (System.IO.File.Exists(_run.Model.ConfigFilePath))
                {
                    ConfigFilePath = _run.Model.ConfigFilePath;
                }
                OnPropertyChanged();
            }
        }
        private RunMATLAB.ViewModels.MatLabEngine _engine;
        public RunMATLAB.ViewModels.MatLabEngine Engine
        {
            get { return _engine; }
        }
        private List<WindRampEvent> _models;
        public List<WindRampEvent> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                _results.Clear();
                _filteredResults.Clear();
                _wrSignals.Clear();
                //var rand = new Random();
                //var colors = Enum.GetValues(typeof(KnownColor));
                //KnownColor[] allColors = new KnownColor[colors.Length];
                //Array.Copy(colors, allColors, colors.Length);
                //var randColor = new KnownColor();
                var randColor = "";
                var signalCounter = 0;
                foreach (var model in value)
                {
                    if (!_wrSignals.ContainsKey(model.PMU + model.Channel))
                    {
                        randColor = Utility.SaturatedColors[signalCounter % 20];
                        _wrSignals[model.PMU + model.Channel] = randColor;
                        signalCounter++;
                    }
                    //var newWREvent = new WindRampEventViewModel(model, randColor);
                    var newWREvent = new WindRampEventViewModel(model, _wrSignals[model.PMU + model.Channel]);
                    //newWREvent.SignalSelectionChanged += _sortSignalSelection;
                    _results.Add(newWREvent);
                    _filteredResults.Add(newWREvent);
                }
                //_drawWREventPlots();
                OnPropertyChanged();
            }
        }

        private Dictionary<string, string> _wrSignals;

        private System.Collections.ObjectModel.ObservableCollection<WindRampEventViewModel> _results;
        private ObservableCollection<WindRampEventViewModel> _filteredResults;
        public ObservableCollection<WindRampEventViewModel> FilteredResults
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
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(_selectedEndTime))
                {
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
            var oldSelection = SelectedWREvent;
            ObservableCollection<WindRampEventViewModel> newResults = new ObservableCollection<WindRampEventViewModel>();
            DateTime startT = DateTime.Parse(_selectedStartTime);
            DateTime endT = DateTime.Parse(_selectedEndTime);
            foreach (var evnt in _results)
            {
                DateTime st = DateTime.Parse(evnt.TrendStart);
                DateTime ed = DateTime.Parse(evnt.TrendEnd);
                if (DateTime.Compare(st, endT) <= 0 && DateTime.Compare(ed, startT) >= 0)
                {
                    newResults.Add(evnt);
                }
            }
            FilteredResults = new ObservableCollection<WindRampEventViewModel>(newResults.OrderBy(x => x.TrendStart));
            if (oldSelection != null)
            {
                var foundSelection = false;
                foreach (var item in FilteredResults)
                {
                    if (item.ID == oldSelection.ID)
                    {
                        SelectedWREvent = item;
                        foundSelection = true;
                        break;
                    }
                }
                if (!foundSelection)
                {
                    SelectedWREvent = FilteredResults.FirstOrDefault();
                }
            }
            //if (FilteredResults.Count() > 0)
            //{
            //    _drawWREventPlots(null);
            //}
        }
        private ObservableCollection<SparsePlot> _sparsePlotModels;
        public ObservableCollection<SparsePlot> SparsePlotModels
        {
            get { return _sparsePlotModels; }
            set
            {
                _sparsePlotModels = value;
                OnPropertyChanged();
            }
        }
        private ViewResolvingPlotModel _wrEventPlotModel;
        public ViewResolvingPlotModel WREventPlotModel
        {
            get { return _wrEventPlotModel; }
            set
            {
                _wrEventPlotModel = value;
                OnPropertyChanged();
            }
        }
        private WindRampEventViewModel _selectedWREvent;
        public WindRampEventViewModel SelectedWREvent
        {
            get { return _selectedWREvent; }
            set
            {
                _selectedWREvent = value;
                _highlightSelectedEvent();
                OnPropertyChanged();
            }
        }

        private void _highlightSelectedEvent()
        {
            if (SelectedWREvent != null && SparsePlotModels.Count() > 0)
            {
                var lowerRange = DateTime.ParseExact(_selectedWREvent.TrendStart, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                var higherRange = DateTime.ParseExact(_selectedWREvent.TrendEnd, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                double axisMin = 0d, axisMax = 0d;
                foreach (var axis in SparsePlotModels.FirstOrDefault().SparsePlotModel.Axes)
                {
                    if (axis.IsVertical())
                    {
                        axisMin = axis.Minimum;
                        axisMax = axis.Maximum;
                    }
                }
                var plotCount = SparsePlotModels.Count();
                var annotations = new List<OxyPlot.Annotations.RectangleAnnotation>();
                for (int i = 0; i < plotCount; i++)
                {
                    var rectAnnotation = new OxyPlot.Annotations.RectangleAnnotation()
                    {
                        Fill = OxyColor.FromArgb(75, 255, 0, 0),
                        //MinimumX = lowerRange,
                        //MaximumX = higherRange,
                        //Fill = OxyColors.Red,
                        MinimumX = lowerRange,
                        MaximumX = higherRange,
                        MinimumY = axisMin,
                        MaximumY = axisMax
                    };
                    annotations.Add(rectAnnotation);
                }
                //WREventPlotModel.Annotations.Clear();
                //WREventPlotModel.Annotations.Add(rectAnnotation);
                //WREventPlotModel.InvalidatePlot(true);
                for (int i = 0; i < plotCount; i++)
                {
                    var detector = SparsePlotModels[i].SparsePlotModel;
                    detector.Annotations.Clear();
                    detector.Annotations.Add(annotations[i]);
                    detector.InvalidatePlot(true);

                }
                //foreach (var detector in SparsePlotModels)
                //{
                //    detector.SparsePlotModel.Annotations.Clear();
                //    detector.SparsePlotModel.Annotations.Add(rectAnnotation);
                //    detector.SparsePlotModel.InvalidatePlot(true);
                //}
            }
        }
        public ICommand PlotWindRampEvent { get; set; }
        private void _drawWREventPlots(object obj)
        {
            var a = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };
            var xAxisFormatString = "";
            var startTime = new DateTime();
            var endTime = new DateTime();
            if (FilteredResults.Count > 0)
            {
                startTime = Convert.ToDateTime(FilteredResults.Min(x => x.TrendStart));
                endTime = Convert.ToDateTime(FilteredResults.Max(x => x.TrendEnd));
            }
            //SelectedStartTime = startTime.ToString("MM/dd/yyyy HH:mm:ss");
            //SelectedEndTime = endTime.ToString("MM/dd/yyyy HH:mm:ss");
            var time = endTime - startTime;
            if (time < TimeSpan.FromHours(24))
            {
                xAxisFormatString = "HH:mm";
            }
            else if (time >= TimeSpan.FromHours(24) && time < TimeSpan.FromHours(168))
            {
                xAxisFormatString = "MM/dd\nHH:mm";
            }
            else
            {
                xAxisFormatString = "MM/dd";
            }
            OxyPlot.Axes.DateTimeAxis timeXAxis = new OxyPlot.Axes.DateTimeAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                MinorIntervalType = OxyPlot.Axes.DateTimeIntervalType.Auto,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true,
                StringFormat = xAxisFormatString,
            };
            timeXAxis.AxisChanged += TimeXAxis_AxisChanged;
            a.Axes.Add(timeXAxis);
            OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Load (MW)",
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true
            };

            double axisMax = FilteredResults.Max(x => x.GetHigherValue()) + (double)0.1;
            double axisMin = FilteredResults.Min(x => x.GetLowerValue()) - (double)0.1;
            if (FilteredResults.Count > 0)
            {
                yAxis.Maximum = axisMax;
                yAxis.Minimum = axisMin;
            }
            a.Axes.Add(yAxis);
            //var wrsignals2 = new List<string>();
            foreach (var wr in FilteredResults)
            {
                if (wr.IsSignalSelected)
                {
                    var newSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
                    newSeries.Points.Add(new DataPoint(Convert.ToDateTime(wr.TrendStart).ToOADate(), wr.ValueStart));
                    newSeries.Points.Add(new DataPoint(Convert.ToDateTime(wr.TrendEnd).ToOADate(), wr.ValueEnd));
                    newSeries.Color = wr.SignalColor;
                    //if (!wrsignals2.Contains(wr.PMU + wr.Channel))
                    //{
                    //wrsignals2.Add(wr.PMU + wr.Channel);
                    newSeries.Title = wr.PMU + " " + wr.Channel;
                    //}
                    newSeries.TrackerFormatString = "{0}";
                    a.Series.Add(newSeries);
                }
            }
            //a.LegendPlacement = LegendPlacement.Outside;
            //a.LegendPosition = LegendPosition.RightMiddle;
            //a.LegendPadding = 0.0;
            //a.LegendSymbolMargin = 0.0;
            //a.LegendMargin = 0;
            //a.LegendMaxWidth = 250;
            a.IsLegendVisible = false;
            //var currentArea = a.LegendArea;
            var currentPlotWithAxis = a.PlotAndAxisArea;

            var currentMargins = a.PlotMargins;
            a.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);
            WREventPlotModel = a;
            _highlightSelectedEvent();
        }

        public ICommand SignalSelectionChanged { get; set; }
        private void _sortSignalSelection(object obj)
        {
            var checkStatus = (bool)obj;
            foreach (var item in _results)
            {
                if (item.Channel == SelectedWREvent.Channel && item.PMU == SelectedWREvent.PMU)
                {
                    item.IsSignalSelected = checkStatus;
                }
            }
            //_drawWREventPlots(null);
            //_highlightSelectedEvent();
            foreach (var item in SparsePlotModels)
            {
                foreach (var item2 in item.SparsePlotModel.Series)
                {
                    if (item2.TrackerKey == SelectedWREvent.PMU + SelectedWREvent.Channel)
                    {
                        item2.IsVisible = checkStatus;
                    }
                }
                item.SparsePlotModel.InvalidatePlot(false);
            }
            //foreach (var detector in SparseResults)
            //{
            //    foreach (var signal in detector.SparseSignals)
            //    {
            //        if (signal.SignalName == SelectedWREvent.Channel && signal.PMUname == SelectedWREvent.PMU)
            //        {

            //        }
            //    }
            //}
        }

        private void TimeXAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            var xAxis = sender as OxyPlot.Axes.DateTimeAxis;
            SelectedStartTime = DateTime.FromOADate(xAxis.ActualMinimum).ToString("MM/dd/yyyy HH:mm:ss");
            SelectedEndTime = DateTime.FromOADate(xAxis.ActualMaximum).ToString("MM/dd/yyyy HH:mm:ss");            
        }

        private List<SparseDetector> _sparseResults;
        public List<SparseDetector> SparseResults
        {
            get { return _sparseResults; }
            set
            {
                _sparseResults = value;
                if (_sparseResults.Count() != 0)
                {
                    _drawWRSparsePlots();
                    if (FilteredResults.Count > 0)
                    {
                        SelectedWREvent = FilteredResults.FirstOrDefault();
                    }
                }
                OnPropertyChanged();
            }
        }
        public ICommand RunSparseMode { get; set; }
        private void _runSparseMode(object obj)
        {
            if (File.Exists(_configFilePath))
            {
                var startTime = Convert.ToDateTime(SelectedStartTime);
                var endTime = Convert.ToDateTime(SelectedEndTime);
                if (startTime <= endTime)
                {
                    try
                    {
                        SparseResults = _engine.GetSparseData(SelectedStartTime, SelectedEndTime, Run, "WindRamp");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Selected start time is later than end time.", "Error!", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Configuration file not found.", "Error!", MessageBoxButtons.OK);
            }
        }


        private void _drawWRSparsePlots()
        {
            var wrPlots = new ObservableCollection<SparsePlot>();
            foreach (var detector in SparseResults)
            {
                var aPlot = new SparsePlot();
                aPlot.Label = detector.Label;
                var a = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };
                //{ PlotAreaBackground = OxyColors.WhiteSmoke}
                //a.PlotType = PlotType.Cartesian;
                var xAxisFormatString = "";
                var startTime = new DateTime();
                var endTime = new DateTime();
                if (detector.SparseSignals.Count > 0)
                {
                    var signal1 = detector.SparseSignals.FirstOrDefault();
                    if (signal1.TimeStamps.Count >= 2)
                    {
                        var timeInterval = signal1.TimeStamps[1] - signal1.TimeStamps[0];
                        startTime = detector.SparseSignals.Min(x => x.TimeStamps.FirstOrDefault()) - timeInterval;
                    }
                    else
                    {
                        startTime = detector.SparseSignals.Min(x => x.TimeStamps.FirstOrDefault());
                    }
                    endTime = detector.SparseSignals.Max(x => x.TimeStamps.LastOrDefault());
                }
                var time = endTime - startTime;
                if (time < TimeSpan.FromHours(24))
                {
                    xAxisFormatString = "HH:mm";
                }
                else if (time >= TimeSpan.FromHours(24) && time < TimeSpan.FromHours(168))
                {
                    xAxisFormatString = "MM/dd\nHH:mm";
                }
                else
                {
                    xAxisFormatString = "MM/dd";
                }
                OxyPlot.Axes.DateTimeAxis timeXAxis = new OxyPlot.Axes.DateTimeAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Bottom,
                    MinorIntervalType = OxyPlot.Axes.DateTimeIntervalType.Auto,
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorGridlineStyle = LineStyle.Dot,
                    MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                    TicklineColor = OxyColor.FromRgb(82, 82, 82),
                    //Title = "Time",
                    IsZoomEnabled = true,
                    IsPanEnabled = true,
                    StringFormat = xAxisFormatString,
                };
                timeXAxis.AxisChanged += TimeXAxis_AxisChanged2;
                a.Axes.Add(timeXAxis);
                OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    Title = detector.Type + "( " + detector.Unit + " )",
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorGridlineStyle = LineStyle.Dot,
                    MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                    TicklineColor = OxyColor.FromRgb(82, 82, 82),
                    IsZoomEnabled = true,
                    IsPanEnabled = true
                };
                yAxis.AxisChanged += YAxis_AxisChanged2;

                double axisMax = detector.SparseSignals.Max(x => x.GetMaxOfMaximum()) + (double)0.1;
                double axisMin = detector.SparseSignals.Min(x => x.GetMinOfMinimum()) - (double)0.1;
                if (SparseResults.Count > 0)
                {
                    yAxis.Maximum = axisMax;
                    yAxis.Minimum = axisMin;
                }
                a.Axes.Add(yAxis);

                foreach (var wr in detector.SparseSignals)
                {
                    var newSeries = new OxyPlot.Series.AreaSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
                    var previousTime = startTime.ToOADate();
                    for (int i = 0; i < wr.Maximum.Count; i++)
                    {
                        newSeries.Points.Add(new DataPoint(previousTime, wr.Maximum[i]));
                        newSeries.Points.Add(new DataPoint(wr.TimeStampNumber[i], wr.Maximum[i]));
                        newSeries.Points2.Add(new DataPoint(previousTime, wr.Minimum[i]));
                        newSeries.Points2.Add(new DataPoint(wr.TimeStampNumber[i], wr.Minimum[i]));
                        previousTime = wr.TimeStampNumber[i];
                    }
                    newSeries.Title = wr.SignalName;
                    newSeries.TrackerFormatString = "{0}";
                    if (FilteredResults.Count > 0)
                    {
                        var foundSignal = FilteredResults.Where(x => x.Channel == wr.SignalName && wr.PMUname == x.PMU).FirstOrDefault();
                        if (foundSignal != null)
                        {
                            var color = foundSignal.SignalColor;
                            newSeries.Color = color;
                            newSeries.Color2 = color;
                            newSeries.Fill = OxyColor.FromAColor(50, color);//, Fill = OxyColor.FromArgb(50, 0, 50, 0)
                        }
                    }
                    newSeries.TrackerKey = wr.Label;
                    //newSeries.MouseMove += RdSparseSeries_MouseMove;
                    //newSeries.MouseDown += RdSparseSeries_MouseDown;
                    a.Series.Add(newSeries);
                }

                a.LegendPlacement = LegendPlacement.Outside;
                a.LegendPosition = LegendPosition.RightMiddle;
                a.LegendPadding = 0.0;
                a.LegendSymbolMargin = 0.0;
                a.LegendMargin = 0;
                //a.LegendMaxHeight = 200;
                a.LegendMaxWidth = 250;

                var currentArea = a.LegendArea;
                var currentPlotWithAxis = a.PlotAndAxisArea;

                var currentMargins = a.PlotMargins;
                a.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);
                aPlot.SparsePlotModel = a;
                wrPlots.Add(aPlot);
            }
            SparsePlotModels = wrPlots;

        }
        private void TimeXAxis_AxisChanged2(object sender, AxisChangedEventArgs e)
        {
            var xAxis = sender as OxyPlot.Axes.DateTimeAxis;
            SelectedStartTime = DateTime.FromOADate(xAxis.ActualMinimum).ToString("MM/dd/yyyy HH:mm:ss");
            SelectedEndTime = DateTime.FromOADate(xAxis.ActualMaximum).ToString("MM/dd/yyyy HH:mm:ss");
            foreach (var plot in SparsePlotModels)
            {
                //OxyPlot.Axes.DateTimeAxis oldAxis = null;
                foreach (var axis in plot.SparsePlotModel.Axes)
                {
                    if (axis.IsHorizontal() && axis.ActualMinimum != xAxis.ActualMinimum)
                    {
                        //oldAxis = axis as OxyPlot.Axes.DateTimeAxis;
                        axis.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                        //axis.Maximum = xAxis.ActualMaximum;
                        plot.SparsePlotModel.InvalidatePlot(false);
                        break;
                    }
                }
                //if (oldAxis!=null)
                //{
                //    plot.SparsePlotModel.Axes.Remove(oldAxis);
                //    var newAxis = xAxis;
                //    plot.SparsePlotModel.Axes.Add(newAxis);
                //    plot.SparsePlotModel.InvalidatePlot(false);
                //}

            }
            //Console.WriteLine("x axis changed! do stuff!" + xAxis.ActualMaximum.ToString() + ", " + xAxis.ActualMinimum.ToString());
        }
        private void YAxis_AxisChanged2(object sender, AxisChangedEventArgs e)
        {
            var yAxis = sender as OxyPlot.Axes.LinearAxis;

        }

    }
}
