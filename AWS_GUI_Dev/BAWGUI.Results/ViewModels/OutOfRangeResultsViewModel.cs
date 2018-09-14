using BAWGUI.Core;
using BAWGUI.MATLABRunResults.Models;
using BAWGUI.Results.Models;
using BAWGUI.Results.Views;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using OxyPlot;
using OxyPlot.Annotations;
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

namespace BAWGUI.Results.ViewModels
{
    public class OutOfRangeResultsViewModel:ViewModelBase
    {
        public OutOfRangeResultsViewModel()
        {
            _configFilePath = "";
            _run = new AWRunViewModel();
            _results = new ObservableCollection<OutOfRangeEventViewModel>();
            _filteredResults = new ObservableCollection<OutOfRangeEventViewModel>();
            _models = new List<OutOfRangeEvent>();
            _engine = RunMATLAB.ViewModels.MatLabEngine.Instance;
            RunSparseMode = new RelayCommand(_runSparseMode);
            OutOfRangeReRun = new RelayCommand(_outOfRangeRerun);
            CancelOutOfRangeReRun = new RelayCommand(_cancelOORReRun);
            ExportOutOfRangeReRunData = new RelayCommand(_exportData);
            _sparsePlotModels = new ObservableCollection<SparsePlot>();
            _oorReRunPlotModels = new ObservableCollection<OORReRunPlot>();
            _selectedStartTime = "01/01/0001 00:00:00";
            _selectedEndTime = "01/01/0001 00:00:00";
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

        private List<OutOfRangeEvent> _models;
        public List<OutOfRangeEvent> Models
        {
            get { return _models; }
            set
            {
                _models = value;
                _results.Clear();
                _filteredResults.Clear();
                foreach (var model in value)
                {
                    _results.Add(new OutOfRangeEventViewModel(model));
                    _filteredResults.Add(new OutOfRangeEventViewModel(model));
                }
                OnPropertyChanged();
            }
        }
        private RunMATLAB.ViewModels.MatLabEngine _engine;
        public RunMATLAB.ViewModels.MatLabEngine Engine
        {
            get { return _engine; }
        }
        private ObservableCollection<OutOfRangeEventViewModel> _results;
        private ObservableCollection<OutOfRangeEventViewModel> _filteredResults;
        public ObservableCollection<OutOfRangeEventViewModel> FilteredResults
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
            ObservableCollection<OutOfRangeEventViewModel> newResults = new ObservableCollection<OutOfRangeEventViewModel>();
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
            _previousSelectedOOREvent = SelectedOOREvent;
            FilteredResults = new ObservableCollection<OutOfRangeEventViewModel>(newResults.OrderBy(x => x.StartTime));
            if (FilteredResults.Contains(_previousSelectedOOREvent))
            {
                SelectedOOREvent = _previousSelectedOOREvent;
            }
            else
            {
                SelectedOOREvent = FilteredResults.FirstOrDefault();
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

        private OutOfRangeEventViewModel _previousSelectedOOREvent;
        private OutOfRangeEventViewModel _selectedOOREvent;
        public OutOfRangeEventViewModel SelectedOOREvent
        {
            get { return _selectedOOREvent; }
            set
            {
                _selectedOOREvent = value;
                if (_selectedOOREvent != null)
                {
                    foreach (var plotM in SparsePlotModels)
                    {
                        var lowerRange = DateTime.ParseExact(_selectedOOREvent.StartTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                        var higherRange = DateTime.ParseExact(_selectedOOREvent.EndTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture).ToOADate();
                        double yaxisMin = 0d, yaxisMax = 0d, xaxisMin = 0d, xaxisMax = 0d;
                        foreach (var axis in plotM.SparsePlotModel.Axes)
                        {
                            if (axis.IsVertical())
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
                            //MinimumX = lowerRange,
                            //MaximumX = higherRange,
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
                        _drawOORSparsePlots();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error plotting data trend of Out of range detector. Original Message:\n" + ex.Message, "Error!", MessageBoxButtons.OK);
                    }
                    if (FilteredResults.Count > 0)
                    {
                        SelectedOOREvent = FilteredResults.FirstOrDefault();
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
                        SparseResults = _engine.GetSparseData(SelectedStartTime, SelectedEndTime, Run, "OutOfRangeGeneral");
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
        private void _drawOORSparsePlots()
        {
            var oorPlots = new ObservableCollection<SparsePlot>();
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

                var currentArea = a.LegendArea;
                var currentPlotWithAxis = a.PlotAndAxisArea;

                var currentMargins = a.PlotMargins;
                a.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);
                aPlot.SparsePlotModel = a;
                aPlot.SparsePlotLegend = sparsePlotLegend;
                oorPlots.Add(aPlot);
            }
            SparsePlotModels = oorPlots;
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
        }

        public ICommand OutOfRangeReRun { get; set; }
        private void _outOfRangeRerun(object obj)
        {
            if (File.Exists(_run.Model.ConfigFilePath))
            {
                var startTime = Convert.ToDateTime(SelectedStartTime);
                var endTime = Convert.ToDateTime(SelectedEndTime);
                if (startTime <= endTime)
                {
                    try
                    {
                    _engine.OORReRunCompletedEvent += _oorReRunCompleted;
                    _engine.OutOfRangeRerun(SelectedStartTime, SelectedEndTime, _run);
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
        private void _oorReRunCompleted(object sender, List<OutOfRangeDetector> e)
        {
            ReRunResult = e;
        }
        private List<OutOfRangeDetector> _reRunResult;
        public List<OutOfRangeDetector> ReRunResult
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
            var oorPlots = new ObservableCollection<OORReRunPlot>();
            foreach (var detector in ReRunResult)
            {
                var aDetector = new OORReRunPlot();
                aDetector.Label = detector.Label;
                if (detector.OORSignals.Count > 0)
                {
                    aDetector.IsByDuration = detector.OORSignals.Any(x => x.IsByDuration);
                    aDetector.IsByROC = detector.OORSignals.Any(x => x.IsByROC);
                }
                var allSignalsPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };

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
                allSignalsPlot.Axes.Add(timeXAxis);
                OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    Title = detector.Type,
                    Unit = detector.Unit,
                    TitlePosition = 0.5,
                    ClipTitle = false,
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorGridlineStyle = LineStyle.Dot,
                    MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                    TicklineColor = OxyColor.FromRgb(82, 82, 82),
                    IsZoomEnabled = true,
                    IsPanEnabled = true
                };
                allSignalsPlot.Axes.Add(yAxis);
                foreach (var oor in detector.OORSignals)
                {
                    var signalSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
                    for (int i = 0; i < oor.Data.Count; i++)
                    {
                        signalSeries.Points.Add(new DataPoint(oor.TimeStampNumber[i], oor.Data[i]));
                    }
                    signalSeries.Title = oor.SignalName;
                    signalSeries.TrackerKey = oor.Label;
                    //signalSeries.MouseDown += OORReRunSeries_MouseDown;
                    allSignalsPlot.Series.Add(signalSeries);
                    if (oor.IsByDuration || oor.IsByROC)
                    {
                        var aSignalPlotModel = _drawAOORSignal(oor);
                        var aNewTriple = new PlotModelThumbnailOORTriple();
                        aNewTriple.IsByDuration = oor.IsByDuration;
                        aNewTriple.IsByROC = oor.IsByROC;
                        var pngExporter = new OxyPlot.Wpf.PngExporter { Width = 600, Height = 400, Background = OxyColors.WhiteSmoke };
                        if (oor.IsByDuration)
                        {
                            aSignalPlotModel.OORDurationPlotModel.IsLegendVisible = false;
                            var bitmapSource = pngExporter.ExportToBitmap(aSignalPlotModel.OORDurationPlotModel); //bitmapsource object
                            aNewTriple.Label = oor.SignalName;
                            aNewTriple.Thumbnail = Utility.ResizeBitmapSource(bitmapSource, 80d);
                            aSignalPlotModel.OORDurationPlotModel.IsLegendVisible = true;
                        }
                        else
                        {
                            aSignalPlotModel.OORROCPlotModel.IsLegendVisible = false;
                            var bitmapSource = pngExporter.ExportToBitmap(aSignalPlotModel.OORROCPlotModel); //bitmapsource object
                            aNewTriple.Label = oor.SignalName;
                            aNewTriple.Thumbnail = Utility.ResizeBitmapSource(bitmapSource, 80d);
                            aSignalPlotModel.OORROCPlotModel.IsLegendVisible = true;
                        }
                        aNewTriple.SignalPlotModelTriple = aSignalPlotModel;
                        aDetector.ThumbnailPlots.Add(aNewTriple);
                    }
                }

                allSignalsPlot.LegendPlacement = LegendPlacement.Outside;
                allSignalsPlot.LegendPosition = LegendPosition.RightMiddle;
                allSignalsPlot.LegendPadding = 0.0;
                allSignalsPlot.LegendSymbolMargin = 0.0;
                allSignalsPlot.LegendMargin = 0;

                var currentArea = allSignalsPlot.LegendArea;
                var currentPlotWithAxis = allSignalsPlot.PlotAndAxisArea;

                var currentMargins = allSignalsPlot.PlotMargins;
                allSignalsPlot.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);
                aDetector.OORReRunAllSignalsPlotModel = allSignalsPlot;
                aDetector.SelectedSignalPlotModel = aDetector.ThumbnailPlots.FirstOrDefault();
                oorPlots.Add(aDetector);
            }
            OORReRunPlotModels = oorPlots;
        }

        private OORSignalDurationROCPlotModelTriple _drawAOORSignal(OutOfRangeSignal oorSig)
        {
            var aNewTriple = new OORSignalDurationROCPlotModelTriple();
            var samplingRate = 1d;
            if (oorSig.TimeStampNumber.Count >= 2)
            {
                var deltaT = (oorSig.TimeStampNumber.LastOrDefault() - oorSig.TimeStampNumber[0]) / (oorSig.TimeStampNumber.Count - 1);
                samplingRate = 1 / (deltaT * 24 * 60 * 60);
            }

            var aSignalPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke, Title = oorSig.SignalName };

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
                Title = oorSig.Type,
                Unit = oorSig.Unit,
                TitlePosition = 0.5,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            aSignalPlot.Axes.Add(yAxis);
            var signalSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Blue };
            var upperDurationLimitSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Red };
            var lowerDurationLimitSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Red };
            var upperRateLimitSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Green };
            var lowerRateLimitSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Green };
            for (int i = 0; i < oorSig.Data.Count; i++)
            {
                signalSeries.Points.Add(new DataPoint(oorSig.TimeStampNumber[i], oorSig.Data[i]));
                upperDurationLimitSeries.Points.Add(new DataPoint(oorSig.TimeStampNumber[i], oorSig.DurationMaxMat[i]));
                lowerDurationLimitSeries.Points.Add(new DataPoint(oorSig.TimeStampNumber[i], oorSig.DurationMinMat[i]));
                upperRateLimitSeries.Points.Add(new DataPoint(oorSig.TimeStampNumber[i], oorSig.RateOfChangeMaxMat[i]));
                lowerRateLimitSeries.Points.Add(new DataPoint(oorSig.TimeStampNumber[i], oorSig.RateOfChangeMinMat[i]));
            }
            signalSeries.Title = oorSig.SignalName;
            //signalSeries.TrackerFormatString = "{0}";
            //upperDurationLimitSeries.TrackerFormatString = "{0}";
            //lowerDurationLimitSeries.TrackerFormatString = "{0}";
            //upperRateLimitSeries.TrackerFormatString = "{0}";
            //lowerRateLimitSeries.TrackerFormatString = "{0}";
            if (oorSig.IsByDuration)
            {
                upperDurationLimitSeries.Title = "Duration Limit";
            }
            if (oorSig.IsByROC)
            {
                upperRateLimitSeries.Title = "Rate Limit";
            }
            signalSeries.TrackerKey = oorSig.Label;
            //signalSeries.MouseDown += OORReRunSeries_MouseDown;
            aSignalPlot.Series.Add(signalSeries);
            aSignalPlot.Series.Add(upperDurationLimitSeries);
            aSignalPlot.Series.Add(lowerDurationLimitSeries);
            aSignalPlot.Series.Add(upperRateLimitSeries);
            aSignalPlot.Series.Add(lowerRateLimitSeries);

            aSignalPlot.LegendPlacement = LegendPlacement.Inside;
            aSignalPlot.LegendPosition = LegendPosition.RightTop;
            aSignalPlot.LegendPadding = 0.0;
            aSignalPlot.LegendSymbolMargin = 0.0;
            aSignalPlot.LegendMargin = 2;
            aSignalPlot.LegendBackground = OxyColors.White;

            var currentArea = aSignalPlot.LegendArea;
            var currentPlotWithAxis = aSignalPlot.PlotAndAxisArea;
            var topPlotSize = aSignalPlot.PlotAndAxisArea;
            var currentMargins = aSignalPlot.PlotMargins;
            aSignalPlot.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);

            aNewTriple.OORSignalPlotModel = aSignalPlot;
            if (oorSig.IsByDuration)
            {
                var aDurationPlotModel = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };

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
                };
                timeXAxis2.AxisChanged += ReRunPlotsTimeXAxis_AxisChanged1;
                aDurationPlotModel.Axes.Add(timeXAxis2);
                OxyPlot.Axes.LinearAxis yAxis2 = new OxyPlot.Axes.LinearAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    Title = "Duration",
                    Unit = "second",
                    TitlePosition = 0.5,
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorGridlineStyle = LineStyle.Dot,
                    MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                    TicklineColor = OxyColor.FromRgb(82, 82, 82),
                    IsZoomEnabled = true,
                    IsPanEnabled = true
                };
                aDurationPlotModel.Axes.Add(yAxis2);
                var outsideCountSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Blue };
                var durationSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Red };
                for (int i = 0; i < oorSig.OutsideCount.Count; i++)
                {
                    outsideCountSeries.Points.Add(new DataPoint(oorSig.TimeStampNumber[i], oorSig.OutsideCount[i] / samplingRate));
                    durationSeries.Points.Add(new DataPoint(oorSig.TimeStampNumber[i], oorSig.Duration / samplingRate));
                }
                outsideCountSeries.Title = "Duration";
                durationSeries.Title = "Threshold";
                //outsideCountSeries.TrackerFormatString = "{0}";
                //durationSeries.TrackerFormatString = "{0}";
                //signalSeries.MouseDown += OORReRunSeries_MouseDown;
                aDurationPlotModel.Series.Add(outsideCountSeries);
                aDurationPlotModel.Series.Add(durationSeries);

                aDurationPlotModel.LegendPlacement = LegendPlacement.Inside;
                aDurationPlotModel.LegendPosition = LegendPosition.RightTop;
                aDurationPlotModel.LegendPadding = 0.0;
                aDurationPlotModel.LegendSymbolMargin = 0.0;
                aDurationPlotModel.LegendMargin = 2;
                aDurationPlotModel.LegendBackground = OxyColors.White;

                //currentArea = aDurationPlotModel.LegendArea;
                //currentPlotWithAxis = aDurationPlotModel.PlotAndAxisArea;
                //currentMargins = aDurationPlotModel.PlotMargins;
                //aDurationPlotModel.PlotMargins = new OxyThickness(currentMargins.Left, currentMargins.Top, 5, currentMargins.Bottom);

                aNewTriple.OORDurationPlotModel = aDurationPlotModel;
            }
            if (oorSig.IsByROC)
            {
                var aROCPlotModel = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };

                OxyPlot.Axes.DateTimeAxis timeXAxis3 = new OxyPlot.Axes.DateTimeAxis()
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
                timeXAxis3.AxisChanged += ReRunPlotsTimeXAxis_AxisChanged1;
                aROCPlotModel.Axes.Add(timeXAxis3);
                OxyPlot.Axes.LinearAxis yAxis3 = new OxyPlot.Axes.LinearAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    Title = "ROC",
                    Unit = oorSig.Unit + "/second",
                    TitlePosition = 0.5,
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorGridlineStyle = LineStyle.Dot,
                    MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                    TicklineColor = OxyColor.FromRgb(82, 82, 82),
                    IsZoomEnabled = true,
                    IsPanEnabled = true
                };
                aROCPlotModel.Axes.Add(yAxis3);
                var rateSeries = new OxyPlot.Series.StemSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Blue };
                var rocSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColors.Red };
                for (int i = 0; i < oorSig.Rate.Count; i++)
                {
                    rateSeries.Points.Add(new DataPoint(oorSig.TimeStampNumber[i], oorSig.Rate[i] * samplingRate));
                    rocSeries.Points.Add(new DataPoint(oorSig.TimeStampNumber[i], oorSig.RateOfChange * samplingRate));
                }
                rateSeries.Title = "Rate Of Change";
                rocSeries.Title = "Threshold";
                //signalSeries.MouseDown += OORReRunSeries_MouseDown;
                aROCPlotModel.Series.Add(rateSeries);
                aROCPlotModel.Series.Add(rocSeries);

                aROCPlotModel.LegendPlacement = LegendPlacement.Inside;
                aROCPlotModel.LegendPosition = LegendPosition.RightTop;
                aROCPlotModel.LegendPadding = 0.0;
                aROCPlotModel.LegendSymbolMargin = 0.0;
                aROCPlotModel.LegendMargin = 2;
                aROCPlotModel.LegendBackground = OxyColors.White;

                //currentArea = aROCPlotModel.LegendArea;
                //currentPlotWithAxis = aROCPlotModel.PlotAndAxisArea;
                //currentMargins = aROCPlotModel.PlotMargins;
                //aROCPlotModel.PlotMargins = new OxyThickness(currentMargins.Left, currentMargins.Top, 5, currentMargins.Bottom);

                aNewTriple.OORROCPlotModel = aROCPlotModel;
            }
            return aNewTriple;
        }

        private void ReRunPlotsTimeXAxis_AxisChanged1(object sender, AxisChangedEventArgs e)
        {
            var xAxis = sender as OxyPlot.Axes.DateTimeAxis;
            var parent = xAxis.Parent;
            foreach (var dtr in OORReRunPlotModels)
            {
                foreach (var triple in dtr.ThumbnailPlots)
                {
                    if (triple.SignalPlotModelTriple.OORDurationPlotModel == parent || triple.SignalPlotModelTriple.OORROCPlotModel == parent || triple.SignalPlotModelTriple.OORSignalPlotModel == parent)
                    {
                        foreach (var ax in triple.SignalPlotModelTriple.OORDurationPlotModel.Axes)
                        {
                            if (ax.IsHorizontal() && ax.ActualMinimum != xAxis.ActualMinimum)
                            {
                                ax.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                                break;
                            }
                        }
                        triple.SignalPlotModelTriple.OORDurationPlotModel.InvalidatePlot(false);
                        foreach (var ax in triple.SignalPlotModelTriple.OORROCPlotModel.Axes)
                        {
                            if (ax.IsHorizontal() && ax.ActualMinimum != xAxis.ActualMinimum)
                            {
                                ax.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                                break;
                            }
                        }
                        triple.SignalPlotModelTriple.OORROCPlotModel.InvalidatePlot(false);
                        foreach (var ax in triple.SignalPlotModelTriple.OORSignalPlotModel.Axes)
                        {
                            if (ax.IsHorizontal() && ax.ActualMinimum != xAxis.ActualMinimum)
                            {
                                ax.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                                break;
                            }
                        }
                        triple.SignalPlotModelTriple.OORSignalPlotModel.InvalidatePlot(false);
                    }
                }
            }
        }

        private ObservableCollection<OORReRunPlot> _oorReRunPlotModels;
        public ObservableCollection<OORReRunPlot> OORReRunPlotModels
        {
            get { return _oorReRunPlotModels; }
            set
            {
                _oorReRunPlotModels = value;
                OnPropertyChanged();
            }
        }
        public ICommand CancelOutOfRangeReRun { get; set; }
        private void _cancelOORReRun(object obj)
        {
            _engine.CancelOORReRun(_run);
        }
        private ExportResultsPopup _exportResultsPopup;
        public ICommand ExportOutOfRangeReRunData { get; set; }
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
                MessageBox.Show("Please retrieve detail first before exporting data.", "Warning!", MessageBoxButtons.OK);
            }
        }
        private void _cancelExportData(object sender, EventArgs e)
        {
            _exportResultsPopup.Close();
        }
    }
}
