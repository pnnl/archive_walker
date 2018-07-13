﻿using BAWGUI.Core;
using BAWGUI.MATLABRunResults.Models;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
            _models = new List<VoltageStabilityEvent>();
            _engine = MatLabEngine.Instance;
            RunSparseMode = new RelayCommand(_runSparseMode);
            _isTheveninValidation = true;
            VSReRun = new RelayCommand(_vsRerun);
            CancelVSReRun = new RelayCommand(_cancelVSReRun);
            //OutOfRangeReRun = new RelayCommand(_outOfRangeRerun);
            //CancelOutOfRangeReRun = new RelayCommand(_cancelOORReRun);
            //_sparsePlotModels = new ObservableCollection<SparsePlot>();
            //_oorReRunPlotModels = new ObservableCollection<OORReRunPlot>();
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
                var aPlot = new SparsePlot();
                aPlot.Label = detector.Label;
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
                a.PlotMargins = new OxyThickness(currentMargins.Left, currentMargins.Top, 5, currentMargins.Bottom);
                aPlot.SparsePlotModel = a;
                rdPlots.Add(aPlot);
            }
            SparsePlotModels = rdPlots;
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
        private bool _isTheveninValidation;
        public bool IsTheveninValidation
        {
            get { return _isTheveninValidation; }
            set
            {
                _isTheveninValidation = value;
                OnPropertyChanged();
            }
        }
        private int _predictionDelay;
        public int PredictionDelay
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
            if (File.Exists(_run.Model.ConfigFilePath))
            {
                var startTime = Convert.ToDateTime(SelectedStartTime);
                var endTime = Convert.ToDateTime(SelectedEndTime);
                if (startTime <= endTime)
                {
                    try
                    {
                        _engine.VSReRunCompletedEvent += _vsReRunCompleted;
                        _engine.VSRerun(SelectedStartTime, SelectedEndTime, _run, PredictionDelay);
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
                _drawOORReRunPlots();
                OnPropertyChanged();
            }
        }
        private void _drawOORReRunPlots()
        {
            //var rdPlots = new ObservableCollection<OORReRunPlot>();
            //foreach (var detector in ReRunResult)
            //{
            //    var aDetector = new OORReRunPlot();
            //    aDetector.Label = detector.Label;
            //    if (detector.OORSignals.Count > 0)
            //    {
            //        aDetector.IsByDuration = detector.OORSignals.Any(x => x.IsByDuration);
            //        aDetector.IsByROC = detector.OORSignals.Any(x => x.IsByROC);
            //    }
            //    var allSignalsPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };

            //    OxyPlot.Axes.DateTimeAxis timeXAxis = new OxyPlot.Axes.DateTimeAxis()
            //    {
            //        Position = OxyPlot.Axes.AxisPosition.Bottom,
            //        MinorIntervalType = OxyPlot.Axes.DateTimeIntervalType.Auto,
            //        MajorGridlineStyle = LineStyle.Dot,
            //        MinorGridlineStyle = LineStyle.Dot,
            //        MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
            //        TicklineColor = OxyColor.FromRgb(82, 82, 82),
            //        IsZoomEnabled = true,
            //        IsPanEnabled = true,
            //    };
            //    allSignalsPlot.Axes.Add(timeXAxis);
            //    OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
            //    {
            //        Position = OxyPlot.Axes.AxisPosition.Left,
            //        Title = detector.Type,
            //        Unit = detector.Unit,
            //        TitlePosition = 0.5,
            //        ClipTitle = false,
            //        MajorGridlineStyle = LineStyle.Dot,
            //        MinorGridlineStyle = LineStyle.Dot,
            //        MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
            //        TicklineColor = OxyColor.FromRgb(82, 82, 82),
            //        IsZoomEnabled = true,
            //        IsPanEnabled = true
            //    };
            //    allSignalsPlot.Axes.Add(yAxis);
            //    foreach (var oor in detector.OORSignals)
            //    {
            //        var signalSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
            //        for (int i = 0; i < oor.Data.Count; i++)
            //        {
            //            signalSeries.Points.Add(new DataPoint(oor.TimeStampNumber[i], oor.Data[i]));
            //        }
            //        signalSeries.Title = oor.SignalName;
            //        signalSeries.TrackerKey = oor.Label;
            //        //signalSeries.MouseDown += OORReRunSeries_MouseDown;
            //        allSignalsPlot.Series.Add(signalSeries);
            //        if (oor.IsByDuration || oor.IsByROC)
            //        {
            //            var aSignalPlotModel = _drawAOORSignal(oor);
            //            var aNewTriple = new PlotModelThumbnailOORTriple();
            //            aNewTriple.IsByDuration = oor.IsByDuration;
            //            aNewTriple.IsByROC = oor.IsByROC;
            //            var pngExporter = new OxyPlot.Wpf.PngExporter { Width = 600, Height = 400, Background = OxyColors.WhiteSmoke };
            //            if (oor.IsByDuration)
            //            {
            //                aSignalPlotModel.OORDurationPlotModel.IsLegendVisible = false;
            //                var bitmapSource = pngExporter.ExportToBitmap(aSignalPlotModel.OORDurationPlotModel); //bitmapsource object
            //                aNewTriple.Label = oor.SignalName;
            //                aNewTriple.Thumbnail = Utility.ResizeBitmapSource(bitmapSource, 80d);
            //                aSignalPlotModel.OORDurationPlotModel.IsLegendVisible = true;
            //            }
            //            else
            //            {
            //                aSignalPlotModel.OORROCPlotModel.IsLegendVisible = false;
            //                var bitmapSource = pngExporter.ExportToBitmap(aSignalPlotModel.OORROCPlotModel); //bitmapsource object
            //                aNewTriple.Label = oor.SignalName;
            //                aNewTriple.Thumbnail = Utility.ResizeBitmapSource(bitmapSource, 80d);
            //                aSignalPlotModel.OORROCPlotModel.IsLegendVisible = true;
            //            }
            //            aNewTriple.SignalPlotModelTriple = aSignalPlotModel;
            //            aDetector.ThumbnailPlots.Add(aNewTriple);
            //        }
            //    }

            //    allSignalsPlot.LegendPlacement = LegendPlacement.Outside;
            //    allSignalsPlot.LegendPosition = LegendPosition.RightMiddle;
            //    allSignalsPlot.LegendPadding = 0.0;
            //    allSignalsPlot.LegendSymbolMargin = 0.0;
            //    allSignalsPlot.LegendMargin = 0;

            //    var currentArea = allSignalsPlot.LegendArea;
            //    var currentPlotWithAxis = allSignalsPlot.PlotAndAxisArea;

            //    var currentMargins = allSignalsPlot.PlotMargins;
            //    allSignalsPlot.PlotMargins = new OxyThickness(currentMargins.Left, currentMargins.Top, 5, currentMargins.Bottom);
            //    aDetector.OORReRunAllSignalsPlotModel = allSignalsPlot;
            //    aDetector.SelectedSignalPlotModel = aDetector.ThumbnailPlots.FirstOrDefault();
            //    rdPlots.Add(aDetector);
            //}
            //OORReRunPlotModels = rdPlots;
        }
        public ICommand CancelVSReRun { get; set; }
        private void _cancelVSReRun(object obj)
        {
            _engine.CancelVSReRun(_run);
        }

    }
}
