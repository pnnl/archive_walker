using BAWGUI.Core;
using BAWGUI.MATLABRunResults.Models;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using VoltageStability.MATLABRunResults.Models;
using VoltageStability.Models;

namespace VoltageStability.ViewModels
{
    public class VoltageStabilityResultsViewModel : ViewModelBase
    {
        public VoltageStabilityResultsViewModel()
        {
            //_configFilePath = "";
            _run = new AWRunViewModel();
            _results = new ObservableCollection<VoltageStabilityEventViewModel>();
            _filteredResults = new ObservableCollection<VoltageStabilityEventViewModel>();
            _oorResults = new List<OutOfRangeEvent>();
            _models = new List<VoltageStabilityEvent>();
            _engine = MatLabEngine.Instance;
            RunSparseMode = new RelayCommand(_runSparseMode);
            //_isTheveninValidation = true;
            VSReRun = new RelayCommand(_vsRerun);
            CancelVSReRun = new RelayCommand(_cancelVSReRun);
            _predictionDelay = "0";
            _selectedStartTime = "01/01/0001 00:00:00";
            _selectedEndTime = "01/01/0001 00:00:00";
            //OutOfRangeReRun = new RelayCommand(_outOfRangeRerun);
            //CancelOutOfRangeReRun = new RelayCommand(_cancelOORReRun);
            //_sparsePlotModels = new ObservableCollection<SparsePlot>();
            //_oorReRunPlotModels = new ObservableCollection<OORReRunPlot>();
        }
        private List<OutOfRangeEvent> _oorResults;
        public List<OutOfRangeEvent> OOrResults
        {
            get { return _oorResults; }
            set
            {
                _oorResults = value;
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
                //if (System.IO.File.Exists(_run.Model.ConfigFilePath))
                //{
                //    ConfigFilePath = _run.Model.ConfigFilePath;
                //}
                OnPropertyChanged();
            }
        }

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
        private MatLabEngine _engine;
        public MatLabEngine Engine
        {
            get { return _engine; }
        }
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
            _previousSelectedVSEvent = SelectedVSEvent;
            FilteredResults = new ObservableCollection<VoltageStabilityEventViewModel>(newResults.OrderBy(x => x.StartTime));
            if (FilteredResults.Contains(_previousSelectedVSEvent))
            {
                SelectedVSEvent = _previousSelectedVSEvent;
            }
            else
            {
                SelectedVSEvent = FilteredResults.FirstOrDefault();
            }
        }
        private VoltageStabilityEventViewModel _previousSelectedVSEvent;
        private VoltageStabilityEventViewModel _selectedVSEvent;
        public VoltageStabilityEventViewModel SelectedVSEvent
        {
            get { return _selectedVSEvent; }
            set
            {
                _selectedVSEvent = value;
                if (_selectedVSEvent != null)
                {
                    foreach (var plotM in SparsePlotModels)
                    {
                        var lowerRange = DateTime.ParseExact(_selectedVSEvent.StartTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                        var higherRange = DateTime.ParseExact(_selectedVSEvent.EndTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
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
                        var highlightWidth = (xaxisMax - xaxisMin) * 0.0005;
                        var actualHighlightWidth = higherRange - lowerRange;
                        if (actualHighlightWidth < highlightWidth)
                        {
                            lowerRange = lowerRange - highlightWidth / 2;
                            higherRange = higherRange + highlightWidth / 2;
                        }

                        //double axisMin = 0d, axisMax = 0d;
                        //foreach (var axis in plotM.SparsePlotModel.Axes)
                        //{
                        //    if (axis.IsVertical())
                        //    {
                        //        axisMin = axis.Minimum;
                        //        axisMax = axis.Maximum;
                        //    }
                        //}
                        var rectAnnotation = new OxyPlot.Annotations.RectangleAnnotation()
                        {
                            Fill = OxyColor.FromArgb(75, 255, 0, 0),
                            MinimumX = lowerRange,
                            MaximumX = higherRange,
                            //Fill = OxyColors.Red,
                            //MinimumX = lowerRange - (higherRange - lowerRange),
                            //MaximumX = higherRange + (higherRange - lowerRange),
                            MinimumY = yaxisMin,
                            MaximumY = yaxisMax
                        };
                        plotM.SparsePlotModel.Annotations.Clear();
                        plotM.SparsePlotModel.Annotations.Add(rectAnnotation);
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
                    _drawVSSparsePlots();
                    if (FilteredResults.Count > 0)
                    {
                        SelectedVSEvent = FilteredResults.FirstOrDefault();
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
        private List<SparseDetector> _oorSparseResults;
        public List<SparseDetector> OORSparseResults
        {
            get { return _oorSparseResults; }
            set
            {
                _oorSparseResults = value;
                if (_sparseResults.Count() != 0)
                {
                    _drawOORSparsePlots();
                }
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SparsePlot> _oorSparsePlotModels;
        public ObservableCollection<SparsePlot> OORSparsePlotModels
        {
            get { return _oorSparsePlotModels; }
            set
            {
                _oorSparsePlotModels = value;
                OnPropertyChanged();
            }
        }

        public ICommand RunSparseMode { get; set; }
        private void _runSparseMode(object obj)
        {
            if (File.Exists(_run.Model.ConfigFilePath))
            {
                var startTime = Convert.ToDateTime(SelectedStartTime);
                var endTime = Convert.ToDateTime(SelectedEndTime);
                if (startTime <= endTime)
                {
                    try
                    {
                        SparseResults = _engine.GetSparseData(SelectedStartTime, SelectedEndTime, Run, "Thevenin");
                        OORSparseResults = _engine.GetSparseData(SelectedStartTime, SelectedEndTime, Run, "OutOfRangeGeneral");
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
                //try
                //{
                //    SparseResults = _engine.GetSparseData(SelectedStartTime, SelectedEndTime, Run, "OutOfRangeGeneral");
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                //}
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Configuration file not found.", "Error!", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }
        private void _drawVSSparsePlots()
        {
            var rdPlots = new ObservableCollection<SparsePlot>();
            foreach (var detector in SparseResults)
            {
                var sparsePlotLegend = new List<string>();
                var aPlot = new SparsePlot();
                aPlot.Label = detector.Unit;
                var a = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };
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
                timeXAxis.AxisChanged += TimeXAxis_AxisChanged;
                a.Axes.Add(timeXAxis);
                OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    Title = detector.Type,
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorGridlineStyle = LineStyle.Dot,
                    MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                    TicklineColor = OxyColor.FromRgb(82, 82, 82),
                    IsZoomEnabled = true,
                    IsPanEnabled = true
                };
                //yAxis.AxisChanged += YAxis_AxisChanged;

                double axisMax = detector.SparseSignals.Max(x => x.GetMaxOfMaximum());
                double axisMin = detector.SparseSignals.Min(x => x.GetMinOfMinimum());
                if (SparseResults.Count > 0)
                {
                    yAxis.Maximum = axisMax;
                    yAxis.Minimum = axisMin;
                }
                a.Axes.Add(yAxis);

                foreach (var rd in detector.SparseSignals)
                {
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
                    newSeries.TrackerFormatString = "{0}";
                    sparsePlotLegend.Add(rd.SignalName);
                    //newSeries.MouseMove += RdSparseSeries_MouseMove;
                    //newSeries.MouseDown += RdSparseSeries_MouseDown;
                    a.Series.Add(newSeries);
                }
                //if (SelectedVSEvent != null)
                //{
                //    var lowerRange = DateTime.ParseExact(SelectedVSEvent.StartTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                //    var higherRange = DateTime.ParseExact(SelectedVSEvent.EndTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
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
                //a.PlotMargins = new OxyThickness(70, double.NaN, double.NaN, double.NaN);

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
        private void _drawOORSparsePlots()
        {
            var oorPlots = new ObservableCollection<SparsePlot>();
            foreach (var detector in OORSparseResults)
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

                foreach (var oor in detector.SparseSignals)
                {
                    var newSeries = new OxyPlot.Series.AreaSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColor.FromArgb(50, 0, 150, 0), Color2 = OxyColor.FromArgb(50, 0, 150, 0), Fill = OxyColor.FromArgb(50, 0, 50, 0) };
                    var previousTime = startTime.ToOADate();
                    for (int i = 0; i < oor.Maximum.Count; i++)
                    {
                        newSeries.Points.Add(new DataPoint(previousTime, oor.Maximum[i]));
                        newSeries.Points.Add(new DataPoint(oor.TimeStampNumber[i], oor.Maximum[i]));
                        newSeries.Points2.Add(new DataPoint(previousTime, oor.Minimum[i]));
                        newSeries.Points2.Add(new DataPoint(oor.TimeStampNumber[i], oor.Minimum[i]));
                        previousTime = oor.TimeStampNumber[i];
                    }
                    newSeries.Title = oor.SignalName;
                    newSeries.TrackerFormatString = "{0}";
                    sparsePlotLegend.Add(oor.SignalName);
                    //newSeries.MouseMove += RdSparseSeries_MouseMove;
                    //newSeries.MouseDown += RdSparseSeries_MouseDown;
                    a.Series.Add(newSeries);
                }
                //if (SelectedOOREvent != null)
                //{
                //    var lowerRange = DateTime.ParseExact(SelectedOOREvent.StartTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                //    var higherRange = DateTime.ParseExact(SelectedOOREvent.EndTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
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
                //a.PlotMargins = new OxyThickness(70, double.NaN, double.NaN, double.NaN);
                var currentArea = a.LegendArea;
                var currentPlotWithAxis = a.PlotAndAxisArea;

                var currentMargins = a.PlotMargins;
                a.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);
                aPlot.SparsePlotModel = a;
                aPlot.SparsePlotLegend = sparsePlotLegend;
                oorPlots.Add(aPlot);
            }
            foreach (var plotM in oorPlots)
            {
                plotM.SparsePlotModel.Annotations.Clear();
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
                foreach (var evnt in OOrResults)
                {

                    var lowerRange = DateTime.ParseExact(evnt.Start, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                    var higherRange = DateTime.ParseExact(evnt.End, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();

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
                        //MinimumX = lowerRange,
                        //MaximumX = higherRange,
                        //Fill = OxyColors.Red,
                        MinimumX = lowerRange,
                        MaximumX = higherRange,
                        MinimumY = yaxisMin,
                        MaximumY = yaxisMax
                    };
                    plotM.SparsePlotModel.Annotations.Add(rectAnnotation);
                    plotM.SparsePlotModel.Annotations.Add(lineAnnotation);
                }
                plotM.SparsePlotModel.InvalidatePlot(true);
            }
            OORSparsePlotModels = oorPlots;
        }
        private void TimeXAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            var xAxis = sender as OxyPlot.Axes.DateTimeAxis;
            SelectedStartTime = DateTime.FromOADate(xAxis.ActualMinimum).ToString("MM/dd/yyyy HH:mm:ss");
            SelectedEndTime = DateTime.FromOADate(xAxis.ActualMaximum).ToString("MM/dd/yyyy HH:mm:ss");
            foreach (var plot in SparsePlotModels)
            {
                foreach (var axis in plot.SparsePlotModel.Axes)
                {
                    if (axis.IsHorizontal() && axis.ActualMinimum != xAxis.ActualMinimum)
                    {
                        axis.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                        plot.SparsePlotModel.InvalidatePlot(false);
                        break;
                    }
                }
            }
            foreach (var plot in OORSparsePlotModels)
            {
                foreach (var axis in plot.SparsePlotModel.Axes)
                {
                    if (axis.IsHorizontal() && axis.ActualMinimum != xAxis.ActualMinimum)
                    {
                        axis.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                        plot.SparsePlotModel.InvalidatePlot(false);
                        break;
                    }
                }
            }
        }


        //private bool _isTheveninValidation;
        //public bool IsTheveninValidation
        //{
        //    get { return _isTheveninValidation; }
        //    set
        //    {
        //        _isTheveninValidation = value;
        //        OnPropertyChanged();
        //    }
        //}
        private string _predictionDelay;
        public string PredictionDelay
        {
            get { return _predictionDelay; }
            set
            {
                _predictionDelay = value;
                OnPropertyChanged();
            }
        }
        public ICommand VSReRun { get; set; }

        private void _vsRerun(object obj)
        {
            var predictionDelay = 0;
            try
            {
                predictionDelay = Int32.Parse(PredictionDelay);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Prediction Delay has to be an positive integer. Original message: " + ex.Message, "Error!", MessageBoxButtons.OK);
                return;
            }
            if (File.Exists(_run.Model.ConfigFilePath))
            {
                var startTime = Convert.ToDateTime(SelectedStartTime);
                var endTime = Convert.ToDateTime(SelectedEndTime);
                if (startTime <= endTime)
                {
                    try
                    {
                        _engine.VSReRunCompletedEvent += _vsReRunCompleted;
                        _engine.VSRerun(SelectedStartTime, SelectedEndTime, _run, predictionDelay);
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
                System.Windows.Forms.MessageBox.Show("Config file not found. Cannot re-run Out of Range", "Error!", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }

        private void _vsReRunCompleted(object sender, List<TheveninDetector> e)
        {
            ReRunResult = e;
        }
        private List<TheveninDetector> _reRunResult;
        public List<TheveninDetector> ReRunResult
        {
            get { return _reRunResult; }
            set
            {
                _reRunResult = value;
                _drawVSReRunPlots();
                OnPropertyChanged();
            }
        }
        private void _drawVSReRunPlots()
        {
            var vsPlots = new ObservableCollection<VSReRunPlot>();
            foreach (var detector in ReRunResult)
            {
                var aDetector = new VSReRunPlot();
                aDetector.Label = detector.SiteName;
                foreach (var signal in detector.TheveninSignals)
                {
                    var aSignalThumbnailPair = _drawAVSSignal(signal);
                    aDetector.ThumbnailPlots.Add(aSignalThumbnailPair);
                }
                aDetector.SelectedSignalPlotModel = aDetector.ThumbnailPlots.FirstOrDefault();
                vsPlots.Add(aDetector);
            }
            VSReRunPlotModels = vsPlots;
        }

        private PlotModelThumbnailVSPair _drawAVSSignal(TheveninSignal signal)
        {
            var aNewPair = new PlotModelThumbnailVSPair();
            aNewPair.Label = signal.Method;
            var aSignalPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };
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
            };
            timeXAxis.AxisChanged += ReRunPlotsTimeXAxis_AxisChanged1;
            aSignalPlot.Axes.Add(timeXAxis);
            OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Bus",
                Unit = "Voltage",
                TitlePosition = 0.5,
                ClipTitle = false,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            aSignalPlot.Axes.Add(yAxis);
            var VBusMagSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
            for (int i = 0; i < signal.VbusMAG.Count; i++)
            {
                VBusMagSeries.Points.Add(new DataPoint(signal.TimeStampNumber[i], signal.VbusMAG[i]));
            }
            VBusMagSeries.Title = "Measured";
            aSignalPlot.Series.Add(VBusMagSeries);
            var VhatSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
            for (int i = 0; i < signal.VhatReal.Count; i++)
            {
                VhatSeries.Points.Add(new DataPoint(signal.TimeStampNumber[i], Math.Sqrt(Math.Pow(signal.VhatReal[i], 2) + Math.Pow(signal.VhatImage[i], 2))));
            }
            VhatSeries.Title = "Estimate";
            aSignalPlot.Series.Add(VhatSeries);


            aSignalPlot.LegendPlacement = LegendPlacement.Inside;
            aSignalPlot.LegendPosition = LegendPosition.RightBottom;
            aSignalPlot.LegendPadding = 0.0;
            aSignalPlot.LegendSymbolMargin = 0.0;
            aSignalPlot.LegendMargin = 0;
            //aSignalPlot.PlotMargins = new OxyThickness(70, double.NaN, double.NaN, double.NaN);

            var currentArea = aSignalPlot.LegendArea;
            var currentMargins = aSignalPlot.PlotMargins;
            aSignalPlot.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);


            aNewPair.VSSignalPlotModel = aSignalPlot;
            var pngExporter = new OxyPlot.Wpf.PngExporter { Width = 600, Height = 400, Background = OxyColors.WhiteSmoke };
            aSignalPlot.IsLegendVisible = false;
            var bitmapSource = pngExporter.ExportToBitmap(aSignalPlot); //bitmapsource object
            aNewPair.Thumbnail = Utility.ResizeBitmapSource(bitmapSource, 80d);
            aSignalPlot.IsLegendVisible = true;

            return aNewPair;
        }

        private void ReRunPlotsTimeXAxis_AxisChanged1(object sender, AxisChangedEventArgs e)
        {
            var xAxis = sender as OxyPlot.Axes.DateTimeAxis;
            var parent = xAxis.Parent;
            foreach (var dtr in VSReRunPlotModels)
            {
                foreach (var pair in dtr.ThumbnailPlots)
                {
                    if (pair.VSSignalPlotModel != parent)
                    {
                        foreach (var ax in pair.VSSignalPlotModel.Axes)
                        {
                            if (ax.IsHorizontal() && ax.ActualMinimum != xAxis.ActualMinimum)
                            {
                                ax.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                                break;
                            }
                        }
                        pair.VSSignalPlotModel.InvalidatePlot(false);
                    }
                }
            }
        }

        public ICommand CancelVSReRun { get; set; }
        private void _cancelVSReRun(object obj)
        {
            _engine.CancelVSReRun(_run);
        }
        private ObservableCollection<VSReRunPlot> _vsReRunPlotModels;
        public ObservableCollection<VSReRunPlot> VSReRunPlotModels
        {
            get { return _vsReRunPlotModels; }
            set
            {
                _vsReRunPlotModels = value;
                OnPropertyChanged();
            }
        }

    }
}
