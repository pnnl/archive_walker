using BAWGUI.Results.Models;
using BAWGUI.RunMATLAB.ViewModels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BAWGUI.Utilities;
using BAWGUI.Core;
using BAWGUI.MATLABRunResults.Models;
using OxyPlot.Annotations;
using BAWGUI.Results.Views;
using System.Windows.Forms;

namespace BAWGUI.Results.ViewModels
{
    public class RingdownResultsViewModel:ViewModelBase
    {
        public RingdownResultsViewModel()
        {
            _results = new ObservableCollection<RingdownEventViewModel>();
            _filteredResults = new ObservableCollection<RingdownEventViewModel>();
            _models = new List<RingDownEvent>();
            _sparseResults = new List<SparseDetector>();
            _sparsePlotModels = new ObservableCollection<SparsePlot>();
            _rdReRunPlotModels = new ObservableCollection<RDreRunPlot>();
            RunSparseMode = new RelayCommand(_runSparseMode);
            RingdownReRun = new RelayCommand(_ringdownRerun);
            CancelRingdownReRun = new RelayCommand(_cancelRDReRun);
            _engine = RunMATLAB.ViewModels.MatLabEngine.Instance;
            _configFilePath = "";
            _reRunResult = new List<RingdownDetector>();
            _run = new AWRunViewModel();
            //_selectedStartTime = "01/01/0001 00:00:00";
            //_selectedEndTime = "01/01/0001 00:00:00";
            _selectedStartTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            _selectedEndTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            ExportRDReRunData = new RelayCommand(_exportData);
        }

        private RunMATLAB.ViewModels.MatLabEngine _engine;
        public RunMATLAB.ViewModels.MatLabEngine Engine
        {
            get { return _engine; }
        }
        private ObservableCollection<RingdownEventViewModel> _results;
        private ObservableCollection<RingdownEventViewModel> _filteredResults;
        public ObservableCollection<RingdownEventViewModel> FilteredResults
        {
            get { return _filteredResults; }
            set
            {
                _filteredResults = value;
                OnPropertyChanged();
            }
        }
        private List<RingDownEvent> _models;
        public List<RingDownEvent> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                _results.Clear();
                _filteredResults.Clear();
                foreach (var model in value)
                {
                    _results.Add(new RingdownEventViewModel(model));
                    _filteredResults.Add(new RingdownEventViewModel(model));
                }
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
                    //System.Windows.Forms.MessageBox.Show("Selected start time is later than end time.", "Error!", System.Windows.Forms.MessageBoxButtons.OK);
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
                        //System.Windows.Forms.MessageBox.Show("Selected end time is earlier than start time.", "Error!", System.Windows.Forms.MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void _filterTableByTime()
        {
            ObservableCollection<RingdownEventViewModel> newResults = new ObservableCollection<RingdownEventViewModel>();
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

            _previousSelectedRDEvent = SelectedRingdownEvent;
            FilteredResults = new ObservableCollection<RingdownEventViewModel>(newResults.OrderBy(x => x.StartTime));
            if (FilteredResults.Contains(_previousSelectedRDEvent))
            {
                SelectedRingdownEvent = _previousSelectedRDEvent;
            }
            else
            {
                SelectedRingdownEvent = FilteredResults.FirstOrDefault();
            }
        }
        private RingdownEventViewModel _previousSelectedRDEvent;
        private RingdownEventViewModel _selectedRingdownEvent;
        public RingdownEventViewModel SelectedRingdownEvent
        {
            get { return _selectedRingdownEvent; }
            set
            {
                _selectedRingdownEvent = value;
                if (_selectedRingdownEvent != null)
                {
                    foreach (var plotM in SparsePlotModels)
                    {
                        var lowerRange = DateTime.ParseExact(_selectedRingdownEvent.StartTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                        var higherRange = DateTime.ParseExact(_selectedRingdownEvent.EndTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                        double yaxisMin = 0d, yaxisMax = 0d, xaxisMin = 0d, xaxisMax = 0d;
                        foreach (var axis in plotM.SparsePlotModel.Axes)
                        {
                            if (axis.IsVertical())
                            {
                                yaxisMin = axis.Minimum;
                                yaxisMax = axis.Maximum;
                            }
                            if (axis.IsHorizontal())
                            {
                                xaxisMin = axis.ActualMinimum;
                                xaxisMax = axis.ActualMaximum;
                            }
                        }
                        var lineAnnotation = new OxyPlot.Annotations.LineAnnotation()
                        {
                            Color = OxyColors.Red,
                            Type = LineAnnotationType.Vertical,
                            X = lowerRange,
                            MaximumY = yaxisMax
                        };
                        var highlightWidth = (xaxisMax - xaxisMin) * 0.0005;
                        var actualHighlightWidth = higherRange - lowerRange;
                        if (actualHighlightWidth < highlightWidth)
                        {
                            lowerRange = lowerRange - highlightWidth / 2;
                            higherRange = higherRange + highlightWidth / 2;
                        }
                        var finalRange = higherRange - lowerRange;
                        var rectAnnotation = new OxyPlot.Annotations.RectangleAnnotation()
                        {
                            Fill = OxyColor.FromArgb(75, 255, 0, 0),
                            //Fill = OxyColors.Red,
                            MinimumX = lowerRange,
                            MaximumX = higherRange,
                            MinimumY = yaxisMin,
                            MaximumY = yaxisMax
                        };
                        plotM.SparsePlotModel.Annotations.Clear();
                        plotM.SparsePlotModel.Annotations.Add(rectAnnotation);
                        plotM.SparsePlotModel.Annotations.Add(lineAnnotation);
                        plotM.SparsePlotModel.InvalidatePlot(true);
                    }
                }
                OnPropertyChanged();
            }
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
                    try
                    {
                        _drawRDSparsePlots();
                        if (FilteredResults.Count > 0)
                        {
                            SelectedRingdownEvent = FilteredResults.FirstOrDefault();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error plotting data trend. Original Message:\n" + ex.Message, "Error!", MessageBoxButtons.OK);
                    }
                }
                OnPropertyChanged();
            }
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
        private void _drawRDSparsePlots()
        {
            var rdPlots = new ObservableCollection<SparsePlot>();
            foreach (var detector in SparseResults)
            {
                var sparsePlotLegend = new List<string>();
                var aPlot = new SparsePlot();
                aPlot.Label = detector.Label;
                var a = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };
                //{ PlotAreaBackground = OxyColors.WhiteSmoke}
                //a.PlotType = PlotType.Cartesian;
                var xAxisFormatString = "";
                var startTime = new DateTime();
                var endTime = new DateTime();
                var timeInterval = new TimeSpan();
                if (detector.SparseSignals.Count > 0)
                {
                    var signal1 = detector.SparseSignals.FirstOrDefault();
                    if (signal1.TimeStamps.Count >= 2)
                    {
                        timeInterval = signal1.TimeStamps[1] - signal1.TimeStamps[0];
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
                timeXAxis.AxisChanged += TimeXAxis_AxisChanged;
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
                //yAxis.AxisChanged += YAxis_AxisChanged;

                double axisMax = detector.SparseSignals.Max(x => x.GetMaxOfMaximum()) + (double)0.1;
                double axisMin = detector.SparseSignals.Min(x => x.GetMinOfMinimum()) - (double)0.1;
                if (SparseResults.Count > 0)
                {
                    yAxis.Maximum = axisMax;
                    yAxis.Minimum = axisMin;
                }
                a.Axes.Add(yAxis);

                //OxyPlot.Axes.RangeColorAxis rangeColorAxis = new OxyPlot.Axes.RangeColorAxis()
                //{
                //    Position = OxyPlot.Axes.AxisPosition.Top,
                //    MajorGridlineStyle = LineStyle.None,
                //    MinorGridlineStyle = LineStyle.None,
                    
                //    //MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                //    //TicklineColor = OxyColor.FromRgb(82, 82, 82),
                //    TextColor = OxyColors.Transparent,
                //    //MajorTickSize = 0,
                //    TickStyle = TickStyle.None,
                //    //MinorTickSize = 0,
                //    IsZoomEnabled = true,
                //    IsPanEnabled = true

                //};
                //rangeColorAxis.AddRange(lowerRange, higherRange, OxyColors.LightGray);
                //a.Axes.Add(rangeColorAxis);
                //var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };
                //a.DefaultColors = OxyPalettes.BlueWhiteRed(FilteredResults.Count).Colors;
                //int index = 0;
                //a.DefaultColors.Clear();
                //var alarmSeries = new OxyPlot.Series.ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 4, Title = "Alarms", ColorAxisKey = null };
                //var trackerKey = 0;
                //var highlightSeries = new OxyPlot.Series.AreaSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColor.FromArgb(50, 0, 150, 0), Color2 = OxyColor.FromArgb(50, 0, 150, 0), Fill = OxyColor.FromArgb(50, 0, 50, 0) };
                foreach (var rd in detector.SparseSignals)
                {
                    //OxyColor eventColor = _mapFrequencyToColor(fo.TypicalFrequency);
                    //a.DefaultColors.Add(eventColor);
                    //foreach (var ocur in fo.FilteredOccurrences)
                    //{
                    //var newSeries = new LineSeries { Title = fo.Label };
                    var newSeries = new OxyPlot.Series.AreaSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColor.FromArgb(50, 0, 150, 0), Color2 = OxyColor.FromArgb(50, 0, 150, 0), Fill = OxyColor.FromArgb(50, 0, 50, 0) };
                    var previousTime = startTime.ToOADate();
                    for (int i = 0; i < rd.Maximum.Count; i++)
                    {
                        newSeries.Points.Add(new DataPoint(previousTime, rd.Maximum[i]));
                        newSeries.Points.Add(new DataPoint(rd.TimeStampNumber[i], rd.Maximum[i]));
                        newSeries.Points2.Add(new DataPoint(previousTime, rd.Minimum[i]));
                        newSeries.Points2.Add(new DataPoint(rd.TimeStampNumber[i], rd.Minimum[i]));
                        previousTime = rd.TimeStampNumber[i];
                    }
                    newSeries.Title = rd.SignalName;
                    sparsePlotLegend.Add(rd.SignalName);
                    newSeries.MouseMove += RdSparseSeries_MouseMove;
                    newSeries.MouseDown += RdSparseSeries_MouseDown;
                    newSeries.TrackerFormatString = "{0}";
                    //if (ocur == SelectedOccurrence)
                    //{
                    //    newSeries.StrokeThickness = 10;
                    //}
                    //newSeries.Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start)), ocur.Frequency));
                    //newSeries.Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End)), ocur.Frequency));
                    a.Series.Add(newSeries);
                    //newSeries.TrackerKey = trackerKey.ToString();
                    //ocur.trackerKey = trackerKey;
                    //trackerKey++;
                    //newSeries.MouseDown += foEvent_MouseDown;
                    //if (ocur.Alarm == "YES")
                    //{
                    //    var startPoint = new ScatterPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start)), ocur.Frequency, 4, 0);
                    //    var endPoint = new ScatterPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End)), ocur.Frequency, 4, 0);
                    //    alarmSeries.Points.Add(startPoint);
                    //    alarmSeries.Points.Add(endPoint);
                    //}
                    //}
                }
                //a.Series.Add(alarmSeries);
                //if (a.DefaultColors.Count > 0)
                //{
                //    a.DefaultColors.Add(_mapFrequencyToColor(axisMax));
                //    a.DefaultColors.Add(_mapFrequencyToColor(axisMin));
                //    a.Axes.Add(new LinearColorAxis { Palette = new OxyPalette(a.DefaultColors.OrderBy(x => x.G)), Position = AxisPosition.Right, Minimum = axisMin, Maximum = axisMax, Title = "Frequency (Hz)", MajorStep = 0.2 });
                //}
                //if (SelectedRingdownEvent != null)
                //{
                //    var lowerRange = DateTime.ParseExact(SelectedRingdownEvent.StartTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                //    var higherRange = DateTime.ParseExact(SelectedRingdownEvent.EndTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                //    var rectAnnotation = new OxyPlot.Annotations.RectangleAnnotation()
                //    {
                //        Fill = OxyColor.FromArgb(50, 50, 50, 50),
                //        MinimumX = lowerRange,
                //        MaximumX = higherRange,
                //        MinimumY = axisMin,
                //        MaximumY = axisMax
                //    };
                //    a.Annotations.Add(rectAnnotation);
                //}


                a.LegendPlacement = LegendPlacement.Outside;
                a.LegendPosition = LegendPosition.RightMiddle;
                a.LegendPadding = 0.0;
                a.LegendSymbolMargin = 0.0;
                a.LegendMargin = 0;
                //a.LegendMaxHeight = 200;
                a.LegendMaxWidth = 250;
                a.IsLegendVisible = false;

                var currentArea = a.LegendArea;
                var currentPlotWithAxis = a.PlotAndAxisArea;

                var currentMargins = a.PlotMargins;
                a.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);
                aPlot.SparsePlotModel = a;
                aPlot.SparsePlotLegend = sparsePlotLegend;
                rdPlots.Add(aPlot);
            }
            SparsePlotModels = rdPlots;
        }

        private void RdSparseSeries_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var s = (OxyPlot.Series.AreaSeries)sender;
            Console.WriteLine(s.Title + "clicked");
        }

        private void RdSparseSeries_MouseMove(object sender, OxyMouseEventArgs e)
        {
            var s = (OxyPlot.Series.AreaSeries)sender;
            Console.WriteLine(s.Title + "moved");
        }

        private void YAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            var yAxis = sender as OxyPlot.Axes.LinearAxis;
            foreach (var plot in SparsePlotModels)
            {
                foreach (var axis in plot.SparsePlotModel.Axes)
                {
                    if (axis.IsVertical() && axis.ActualMinimum != yAxis.ActualMinimum)
                    {
                        axis.Zoom(yAxis.ActualMinimum, yAxis.ActualMaximum);
                        plot.SparsePlotModel.InvalidatePlot(false);
                        break;
                    }
                }

            }
            Console.WriteLine("frequency axis changed! do stuff!" + yAxis.ActualMaximum.ToString() + ", " + yAxis.ActualMinimum.ToString());
        }

        private void TimeXAxis_AxisChanged(object sender, AxisChangedEventArgs e)
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
            }
            Console.WriteLine("x axis changed! do stuff!" + xAxis.ActualMaximum.ToString() + ", " + xAxis.ActualMinimum.ToString());
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
                        SparseResults = _engine.GetSparseData(SelectedStartTime, SelectedEndTime, Run, "Ringdown");
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Selected start time is later than end time.", "Error!", System.Windows.Forms.MessageBoxButtons.OK);
                }
                //try
                //{
                //    SparseResults = _engine.GetSparseData(SelectedStartTime, SelectedEndTime, Run, "Ringdown");
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                //}
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Configuration file not found.", "Error!", MessageBoxButtons.OK);
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
        public ICommand RingdownReRun { get; set; }
        private void _ringdownRerun(object obj)
        {
            //first stop background normal run if any
            //start rerun in background
            if (File.Exists(ConfigFilePath))
            {
                var startTime = Convert.ToDateTime(SelectedStartTime);
                var endTime = Convert.ToDateTime(SelectedEndTime);
                if (startTime <= endTime)
                {
                    try
                    {
                        _engine.RDReRunCompletedEvent += _rDReRunCompleted;
                        _engine.RingDownRerun(SelectedStartTime, SelectedEndTime, _run);
                        //ReRunResult = _engine.RDReRunResults;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Selected start time is later than end time.", "Error!", System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Config file not found. Cannot re-run Ringdown", "Error!", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }

        private void _rDReRunCompleted(object sender, List<RingdownDetector> e)
        {
            ReRunResult = e;
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

        private List<RingdownDetector> _reRunResult;
        public List<RingdownDetector> ReRunResult
        {
            get { return _reRunResult; }
            set
            {
                _reRunResult = value;
                _drawRDReRunPlots();
                OnPropertyChanged();
            }
        }
        private ObservableCollection<RDreRunPlot> _rdReRunPlotModels;
        public ObservableCollection<RDreRunPlot> RdReRunPlotModels
        {
            get { return _rdReRunPlotModels; }
            set
            {
                _rdReRunPlotModels = value;
                OnPropertyChanged();
            }
        }
        private void _drawRDReRunPlots()
        {
            var rdPlots = new ObservableCollection<RDreRunPlot>();
            foreach (var detector in ReRunResult)
            {
                var aDetector = new RDreRunPlot();
                aDetector.Label = detector.Label;
                var allSignalsPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };
                ////{ PlotAreaBackground = OxyColors.WhiteSmoke}
                ////a.PlotType = PlotType.Cartesian;
                ////var xAxisFormatString = "";
                ////var startTime = detector.RingdownSignals.Min(x => x.TimeStamps.FirstOrDefault());
                ////var endTime = detector.RingdownSignals.Max(x => x.TimeStamps.LastOrDefault());
                ////var time = endTime - startTime;
                ////if (time < TimeSpan.FromHours(24))
                ////{
                ////    xAxisFormatString = "HH:mm";
                ////}
                ////else if (time >= TimeSpan.FromHours(24) && time < TimeSpan.FromHours(168))
                ////{
                ////    xAxisFormatString = "MM/dd\nHH:mm";
                ////}
                ////else
                ////{
                ////    xAxisFormatString = "MM/dd";
                ////}
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
                    //StringFormat = xAxisFormatString,
                };
                //timeXAxis.AxisChanged += TimeXAxis_AxisChanged;
                allSignalsPlot.Axes.Add(timeXAxis);
                //OxyPlot.Axes.LinearAxis xAxis = new OxyPlot.Axes.LinearAxis()
                //{
                //    Position = OxyPlot.Axes.AxisPosition.Bottom,
                //    MajorGridlineStyle = LineStyle.Dot,
                //    MinorGridlineStyle = LineStyle.Dot,
                //    MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                //    TicklineColor = OxyColor.FromRgb(82, 82, 82),
                //    IsZoomEnabled = true,
                //    IsPanEnabled = true,
                //    StringFormat = null,
                //    LabelFormatter = _convertDoubleToTimeString
                //};
                //allSignalsPlot.Axes.Add(xAxis);
                OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    Title = detector.Type,
                    Unit = detector.Unit,
                    
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorGridlineStyle = LineStyle.Dot,
                    MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                    TicklineColor = OxyColor.FromRgb(82, 82, 82),
                    IsZoomEnabled = true,
                    IsPanEnabled = true
                };
                //yAxis.AxisChanged += YAxis_AxisChanged;

                //double axisMax = detector.RingdownSignals.Max(x => x.GetMaxOfMaximum()) + (double)0.01;
                //double axisMin = detector.RingdownSignals.Min(x => x.GetMinOfMinimum()) - (double)0.01;
                //if (ReRunResult.Count > 0)
                //{
                //    yAxis.Maximum = axisMax;
                //    yAxis.Minimum = axisMin;
                //}
                allSignalsPlot.Axes.Add(yAxis);
                //var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };
                //a.DefaultColors = OxyPalettes.BlueWhiteRed(FilteredResults.Count).Colors;
                //int index = 0;
                //a.DefaultColors.Clear();
                //var alarmSeries = new OxyPlot.Series.ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 4, Title = "Alarms", ColorAxisKey = null };
                //var trackerKey = 0;
                foreach (var rd in detector.RingdownSignals)
                {
                    //OxyColor eventColor = _mapFrequencyToColor(fo.TypicalFrequency);
                    //a.DefaultColors.Add(eventColor);
                    //foreach (var ocur in fo.FilteredOccurrences)
                    //{
                    //var newSeries = new LineSeries { Title = fo.Label };
                    var newSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2};
                    for (int i = 0; i < rd.Data.Count; i++)
                    {
                        newSeries.Points.Add(new DataPoint(rd.TimeStampNumber[i], rd.Data[i]));
                        //newSeries.Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(rd.TimeStamps[i]), rd.Data[i]));
                        //newSeries.Points2.Add(new DataPoint(rd.TimeStampNumber[i], rd.Data[i]));
                    }
                    newSeries.Title = rd.SignalName;
                    newSeries.TrackerKey = rd.Label;
                    //newSeries.MouseMove += RdReRunSeries_MouseMove;
                    newSeries.MouseDown += RdReRunSeries_MouseDown;
                    //if (ocur == SelectedOccurrence)
                    //{
                    //    newSeries.StrokeThickness = 10;
                    //}
                    //newSeries.Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start)), ocur.Frequency));
                    //newSeries.Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End)), ocur.Frequency));
                    allSignalsPlot.Series.Add(newSeries);
                    //ocur.trackerKey = trackerKey;
                    //trackerKey++;
                    //newSeries.MouseDown += foEvent_MouseDown;
                    //if (ocur.Alarm == "YES")
                    //{
                    //    var startPoint = new ScatterPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start)), ocur.Frequency, 4, 0);
                    //    var endPoint = new ScatterPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End)), ocur.Frequency, 4, 0);
                    //    alarmSeries.Points.Add(startPoint);
                    //    alarmSeries.Points.Add(endPoint);
                    //}
                    //}
                    var aSignalPlotModel = _drawARDSignal(rd);
                    var aNewPair = new PlotModelThumbnailPair();
                    var pngExporter = new PngExporter { Width = 600, Height = 400, Background = OxyColors.WhiteSmoke };
                    aSignalPlotModel.RdThresholdRMSPlotModel.IsLegendVisible = false;
                    //var originalTitleFontSize = aSignalPlotModel.RdThresholdRMSPlotModel.TitleFontSize;
                    //aSignalPlotModel.RdThresholdRMSPlotModel.TitleFontSize = 24;
                    //var originalTitleFontWeight = aSignalPlotModel.RdThresholdRMSPlotModel.TitleFontWeight;
                    //aSignalPlotModel.RdThresholdRMSPlotModel.TitleFontWeight = 16;
                    var bitmapSource = pngExporter.ExportToBitmap(aSignalPlotModel.RdThresholdRMSPlotModel); //bitmapsource object
                    aNewPair.Label = rd.SignalName;
                    aNewPair.Thumbnail = Utility.ResizeBitmapSource(bitmapSource, 80d);
                    //aSignalPlotModel.RdThresholdRMSPlotModel.TitleFontSize = originalTitleFontSize;
                    //aSignalPlotModel.RdThresholdRMSPlotModel.TitleFontWeight = originalTitleFontWeight;
                    aSignalPlotModel.RdThresholdRMSPlotModel.IsLegendVisible = true;
                    aNewPair.SignalPlotModelPair = aSignalPlotModel;
                    aDetector.ThumbnailPlots.Add(aNewPair);
                    //aDetector.RDSignalPlotModels.Add(aSignalPlotModel);

                }
                //a.Series.Add(alarmSeries);
                //if (a.DefaultColors.Count > 0)
                //{
                //    a.DefaultColors.Add(_mapFrequencyToColor(axisMax));
                //    a.DefaultColors.Add(_mapFrequencyToColor(axisMin));
                //    a.Axes.Add(new LinearColorAxis { Palette = new OxyPalette(a.DefaultColors.OrderBy(x => x.G)), Position = AxisPosition.Right, Minimum = axisMin, Maximum = axisMax, Title = "Frequency (Hz)", MajorStep = 0.2 });
                //}

                allSignalsPlot.LegendPlacement = LegendPlacement.Outside;
                allSignalsPlot.LegendPosition = LegendPosition.RightMiddle;
                allSignalsPlot.LegendPadding = 0.0;
                allSignalsPlot.LegendSymbolMargin = 0.0;
                allSignalsPlot.LegendMargin = 0;
                allSignalsPlot.LegendMaxWidth = 50;

                var currentArea = allSignalsPlot.LegendArea;
                var currentPlotWithAxis = allSignalsPlot.PlotAndAxisArea;
                var thisPlotHeight = currentPlotWithAxis.Height;
                var thisPlotWidth = currentPlotWithAxis.Width;

                var currentMargins = allSignalsPlot.PlotMargins;
                allSignalsPlot.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);
                aDetector.RDreRunPlotModel = allSignalsPlot;
                aDetector.SelectedSignalPlotModel = aDetector.ThumbnailPlots.FirstOrDefault();
                rdPlots.Add(aDetector);
            }
            RdReRunPlotModels = rdPlots;
        }
        /// <summary>
        /// this function can be implemented if the x axis is going to be linear axis and we manually convert it to a time string to show microseconds
        /// </summary>
        /// <param name = "arg" > double value represent time</param>
        /// <returns></returns>
        //private string _convertDoubleToTimeString(double arg)
        //{
        //    return arg.ToString();
        //}
        private void RdReRunSeries_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var s = (OxyPlot.Series.LineSeries)sender;
            var signalLabel = s.TrackerKey;
            var detectorLabel = signalLabel.Split('s')[0];
            foreach (var detector in ReRunResult)
            {
                if (detector.Label == detectorLabel)
                {
                    foreach (var signal in detector.RingdownSignals)
                    {
                        if (signal.Label == signalLabel)
                        {
                            Console.WriteLine(s.Title + "found");
                            //_drawARDSignal(signal);
                        }
                    }
                }
            }
        }

        private DataThresholdRMSPlotModelPair _drawARDSignal(RingdownSignal signal)
        {
            var aNewPair = new DataThresholdRMSPlotModelPair();
            var aSignalPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke, Title = signal.SignalName };
            //var xAxisFormatString = "";
            //var startTime = signal.TimeStamps.FirstOrDefault();
            //var endTime = signal.TimeStamps.LastOrDefault();
            //var time = endTime - startTime;
            //if (time < TimeSpan.FromHours(24))
            //{
            //    xAxisFormatString = "HH:mm";
            //}
            //else if (time >= TimeSpan.FromHours(24) && time < TimeSpan.FromHours(168))
            //{
            //    xAxisFormatString = "MM/dd\nHH:mm";
            //}
            //else
            //{
            //    xAxisFormatString = "MM/dd";
            //}
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
                //StringFormat = xAxisFormatString,
            };
            timeXAxis.AxisChanged += ReRunPlotsTimeXAxis_AxisChanged;
            aSignalPlot.Axes.Add(timeXAxis);
            OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = signal.Type,
                Unit = signal.Unit,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            aSignalPlot.Axes.Add(yAxis);
            var dataSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2};
            for (int i = 0; i < signal.Data.Count; i++)
            {
                dataSeries.Points.Add(new DataPoint(signal.TimeStampNumber[i], signal.Data[i]));
            }
            dataSeries.Title = "Data";
            aSignalPlot.Series.Add(dataSeries);
            //var thresholdSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2};
            //for (int i = 0; i < signal.Threshold.Count; i++)
            //{
            //    thresholdSeries.Points.Add(new DataPoint(signal.TimeStampNumber[i], signal.Threshold[i]));
            //}
            //thresholdSeries.Title = "Threshold";
            //aSignalPlot.Series.Add(thresholdSeries);
            //var rmsSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2};
            //for (int i = 0; i < signal.TestStatistic.Count; i++)
            //{
            //    rmsSeries.Points.Add(new DataPoint(signal.TimeStampNumber[i], signal.TestStatistic[i]));
            //}
            //rmsSeries.Title = "Test Statistic";
            //aSignalPlot.Series.Add(rmsSeries);
            aSignalPlot.LegendPlacement = LegendPlacement.Inside;
            aSignalPlot.LegendPosition = LegendPosition.RightBottom;
            aSignalPlot.LegendPadding = 0.0;
            aSignalPlot.LegendSymbolMargin = 0.0;
            aSignalPlot.LegendMargin = 2;
            aSignalPlot.LegendBackground = OxyColors.White;

            var currentArea = aSignalPlot.LegendArea;
            var currentPlotWithAxis = aSignalPlot.PlotAndAxisArea;

            var currentMargins = aSignalPlot.PlotMargins;
            aSignalPlot.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);

            aNewPair.RDSignalPlotModel = aSignalPlot;

            var aThresholdRMSPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };
            //xAxisFormatString = "";
            //startTime = signal.TimeStamps.FirstOrDefault();
            //endTime = signal.TimeStamps.LastOrDefault();
            //time = endTime - startTime;
            //if (time < TimeSpan.FromHours(24))
            //{
            //    xAxisFormatString = "HH:mm";
            //}
            //else if (time >= TimeSpan.FromHours(24) && time < TimeSpan.FromHours(168))
            //{
            //    xAxisFormatString = "MM/dd\nHH:mm";
            //}
            //else
            //{
            //    xAxisFormatString = "MM/dd";
            //}
            OxyPlot.Axes.DateTimeAxis timeXAxis2 = new OxyPlot.Axes.DateTimeAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                MinorIntervalType = OxyPlot.Axes.DateTimeIntervalType.Auto,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true,
                //StringFormat = xAxisFormatString,
            };
            timeXAxis2.AxisChanged += ReRunPlotsTimeXAxis_AxisChanged;
            aThresholdRMSPlot.Axes.Add(timeXAxis2);
            OxyPlot.Axes.LinearAxis yAxis2 = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Signal Energy",
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            aThresholdRMSPlot.Axes.Add(yAxis2);
            //var dataSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
            //for (int i = 0; i < signal.Data.Count; i++)
            //{
            //    dataSeries.Points.Add(new DataPoint(signal.TimeStampNumber[i], signal.Data[i]));
            //}
            //dataSeries.Title = "Data";
            //aSignalPlot.Series.Add(dataSeries);
            var thresholdSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Red };
            for (int i = 0; i < signal.Threshold.Count; i++)
            {
                thresholdSeries.Points.Add(new DataPoint(signal.TimeStampNumber[i], signal.Threshold[i]));
            }
            thresholdSeries.Title = "Threshold";
            aThresholdRMSPlot.Series.Add(thresholdSeries);
            var rmsSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Black };
            for (int i = 0; i < signal.TestStatistic.Count; i++)
            {
                rmsSeries.Points.Add(new DataPoint(signal.TimeStampNumber[i], signal.TestStatistic[i]));
            }
            rmsSeries.Title = "Test Statistic";
            aThresholdRMSPlot.Series.Add(rmsSeries);
            aThresholdRMSPlot.LegendPlacement = LegendPlacement.Inside;
            aThresholdRMSPlot.LegendPosition = LegendPosition.BottomRight;
            aThresholdRMSPlot.LegendPadding = 0.0;
            aThresholdRMSPlot.LegendSymbolMargin = 0.0;
            aThresholdRMSPlot.LegendMargin = 2;
            aSignalPlot.LegendBackground = OxyColors.White;

            currentArea = aThresholdRMSPlot.LegendArea;
            currentPlotWithAxis = aThresholdRMSPlot.PlotAndAxisArea;

            currentMargins = aThresholdRMSPlot.PlotMargins;
            aThresholdRMSPlot.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);


            aNewPair.RdThresholdRMSPlotModel = aThresholdRMSPlot;

            return aNewPair;
        }

        private void ReRunPlotsTimeXAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            var xAxis = sender as OxyPlot.Axes.DateTimeAxis;
            var parent = xAxis.Parent;
            var thisPlotModel = xAxis.PlotModel;
            foreach (var dtr in RdReRunPlotModels)
            {
                foreach (var pair in dtr.ThumbnailPlots)
                {
                    if (pair.SignalPlotModelPair.RDSignalPlotModel == parent || pair.SignalPlotModelPair.RdThresholdRMSPlotModel == thisPlotModel)
                    {
                        foreach (var ax in pair.SignalPlotModelPair.RDSignalPlotModel.Axes)
                        {
                            if (ax.IsHorizontal() && ax.ActualMinimum != xAxis.ActualMinimum)
                            {
                                ax.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                                break;
                            }
                        }
                        pair.SignalPlotModelPair.RDSignalPlotModel.InvalidatePlot(false);
                        foreach (var ax in pair.SignalPlotModelPair.RdThresholdRMSPlotModel.Axes)
                        {
                            if (ax.IsHorizontal() && ax.ActualMinimum != xAxis.ActualMinimum)
                            {
                                ax.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                                break;
                            }
                        }
                        pair.SignalPlotModelPair.RdThresholdRMSPlotModel.InvalidatePlot(false);
                    }
                }
            }
        }

        private void RdReRunSeries_MouseMove(object sender, OxyMouseEventArgs e)
        {
            var s = (OxyPlot.Series.LineSeries)sender;
            Console.WriteLine(s.Title + "moved");
        }

        public ICommand CancelRingdownReRun { get; set; }
        private void _cancelRDReRun(object obj)
        {
            //string RunPath = @"C:\Users\wang690\Desktop\projects\ArchiveWalker\RerunTest\Project_RerunTestRD\Run_test\";
            //var controlPath = RunPath + "ControlRerun\\";
            _engine.CancelRDReRun(_run);
            //MessageBox.Show("method is not implemented yet!!!", "ERROR!", MessageBoxButton.OK);
        }
        private ExportResultsPopup _exportResultsPopup;

        public ICommand ExportRDReRunData { get; set; }
        private void _exportData(object obj)
        {
            if (ReRunResult.Any())
            {
                var exportResult = new ResultsExportingViewModel(ReRunResult);
                exportResult.ExportDataCancelled += _cancelExportData;
                _exportResultsPopup = new ExportResultsPopup
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                    DataContext = exportResult
                };
                _exportResultsPopup.ShowDialog();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please retrieve detail first before exporting data.", "Warning!", MessageBoxButtons.OK);
            }
        }
        private void _cancelExportData(object sender, EventArgs e)
        {
            _exportResultsPopup.Close();
        }
    }
}
