using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.MATLABRunResults.Models;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using ModeMeter.MATLABRunResults.Models;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ModeMeter.ViewModels
{
    public class ModeMeterResultsViewModel : ViewModelBase
    {
        private AWRunViewModel _run;
        public AWRunViewModel Run
        {
            get { return _run; }
            set
            {
                if (_run != value)
                {
                    _run = value;
                    GetMostRecentTimeFromEventFolder();
                    OnPropertyChanged();
                }
            }
        }

        private MatLabEngine _engine;
        public MatLabEngine Engine
        {
            get { return _engine; }
        }

        public ModeMeterResultsViewModel()
        {
            _run = new AWRunViewModel();
            _engine = MatLabEngine.Instance;
            RunSparseMode = new RelayCommand(_runSparseMode);
            MMReRun = new RelayCommand(_mmReRun);
            CancelMMReRun = new RelayCommand(_cancelMMReRun);
            SelectedStartTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            SelectedEndTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        }

        public ICommand CancelMMReRun { get; set; }
        private void _cancelMMReRun(object obj)
        {
            throw new NotImplementedException();
        }
        public ICommand MMReRun { get; set; }
        private void _mmReRun(object obj)
        {
            if (File.Exists(_run.Model.ConfigFilePath))
            {
                var startTime = Convert.ToDateTime(SelectedStartTime);
                var endTime = Convert.ToDateTime(SelectedEndTime);
                if (startTime <= endTime)
                {
                    try
                    {
                        _engine.MMReRunCompletedEvent += _mmReRunCompleted;
                        _engine.MMRerun(SelectedStartTime, SelectedEndTime, _run);
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
                        SparseResults = _engine.GetSparseData(SelectedStartTime, SelectedEndTime, Run, "ModeMeter");
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
                    //_filterTableByTime();
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
                        //_filterTableByTime();
                    }
                    else
                    {
                        //MessageBox.Show("Selected end time is earlier than start time.", "Error!", MessageBoxButtons.OK);
                    }
                }
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
                    _drawMMSparsePlots();
                }
                OnPropertyChanged();
            }
        }
        private void _drawMMSparsePlots()
        {
            var mmPlots = new ObservableCollection<SparsePlot>();
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

                foreach (var mm in detector.SparseSignals)
                {
                    var newSeries = new OxyPlot.Series.AreaSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2, Color = OxyColor.FromArgb(50, 0, 150, 0), Color2 = OxyColor.FromArgb(50, 0, 150, 0), Fill = OxyColor.FromArgb(50, 0, 50, 0) };
                    var previousTime = startTime.ToOADate();
                    for (int i = 0; i < mm.Maximum.Count; i++)
                    {
                        newSeries.Points.Add(new DataPoint(previousTime, mm.Maximum[i]));
                        newSeries.Points.Add(new DataPoint(mm.TimeStampNumber[i], mm.Maximum[i]));
                        newSeries.Points2.Add(new DataPoint(previousTime, mm.Minimum[i]));
                        newSeries.Points2.Add(new DataPoint(mm.TimeStampNumber[i], mm.Minimum[i]));
                        previousTime = mm.TimeStampNumber[i];
                    }
                    newSeries.Title = mm.SignalName;
                    newSeries.TrackerFormatString = "{0}";
                    sparsePlotLegend.Add(mm.SignalName);
                    a.Series.Add(newSeries);
                }
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
                mmPlots.Add(aPlot);
            }
            SparsePlotModels = mmPlots;
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
        }

        public void GetMostRecentTimeFromEventFolder()
        {
            if (Run != null && Directory.Exists(Run.Model.EventPath))
            {
                var allName = new List<DateTime>();
                _findAllCSVDataFileNames(Run.Model.EventPath, allName);
                if (allName.Count > 0)
                {
                    var latest = allName.Max();
                    SelectedStartTime = latest.ToString("MM/dd/yyyy HH:mm:ss");
                    SelectedEndTime = latest.AddDays(1).AddSeconds(-1).ToString("MM/dd/yyyy HH:mm:ss");
                }
            }
        }

        private void _findAllCSVDataFileNames(string path, List<DateTime> allName)
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                _findAllCSVDataFileNames(dir, allName);
            }
            foreach (var file in Directory.GetFiles(path))
            {
                var ext = Path.GetExtension(file).ToLower();
                if (ext == ".csv")
                {
                    try
                    {
                        var filename = DateTime.ParseExact(Path.GetFileNameWithoutExtension(file), "yyMMdd", CultureInfo.InvariantCulture);
                        allName.Add(filename);
                    }
                    catch (Exception ex)
                    {
                        //throw;
                    }
                }
            }
        }
        private void _mmReRunCompleted(object sender, List<ModeMeterPlotData> e)
        {
            ReRunResult = e;
        }
        private List<ModeMeterPlotData> _reRunResult;
        public List<ModeMeterPlotData> ReRunResult
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
            var mmrrPlots = new ObservableCollection<MMRerunPlotModel>();
            foreach (var plot in ReRunResult)
            {
                var plotWithLegend = new MMRerunPlotModel();
                var aPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke, Title = plot.Title };
                ObservableCollection<Legend> legends =new ObservableCollection<Legend>();

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
                aPlot.Axes.Add(timeXAxis);
                OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    Title = plot.YLabel,
                    TitlePosition = 0.5,
                    ClipTitle = false,
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorGridlineStyle = LineStyle.Dot,
                    MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                    TicklineColor = OxyColor.FromRgb(82, 82, 82),
                    IsZoomEnabled = true,
                    IsPanEnabled = true
                };
                aPlot.Axes.Add(yAxis);
                var signalCounter = 0;
                foreach (var mm in plot.Signals)
                {
                    var signalSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
                    for (int i = 0; i < mm.Data.Count; i++)
                    {
                        signalSeries.Points.Add(new DataPoint(plot.TimeStampNumber[i], mm.Data[i]));
                    }
                    signalSeries.Title = mm.SignalName;
                    //signalSeries.MouseDown += OORReRunSeries_MouseDown;
                    aPlot.Series.Add(signalSeries);
                    var c = string.Format("#{0:x6}", Color.FromName(Utility.SaturatedColors[signalCounter % 20]).ToArgb());
                    signalSeries.Color = OxyColor.Parse(c);
                    legends.Add(new Legend(mm.SignalName, signalSeries.Color));
                    signalCounter++;
                }

                aPlot.LegendPlacement = LegendPlacement.Outside;
                aPlot.LegendPosition = LegendPosition.RightMiddle;
                aPlot.LegendPadding = 0.0;
                aPlot.LegendSymbolMargin = 0.0;
                aPlot.LegendMargin = 0;
                aPlot.IsLegendVisible = false;

                var currentArea = aPlot.LegendArea;
                var currentPlotWithAxis = aPlot.PlotAndAxisArea;

                var currentMargins = aPlot.PlotMargins;
                aPlot.PlotMargins = new OxyThickness(70, currentMargins.Top, 5, currentMargins.Bottom);
                plotWithLegend.MMReRunAllSignalsPlotModel = aPlot;
                plotWithLegend.MMReRunPlotLegend = legends;
                mmrrPlots.Add(plotWithLegend);
            }
            MMReRunPlotModels = mmrrPlots;
        }
        private ObservableCollection<MMRerunPlotModel> _mmReRunPlotModels;
        public ObservableCollection<MMRerunPlotModel> MMReRunPlotModels
        {
            get { return _mmReRunPlotModels; }
            set
            {
                _mmReRunPlotModels = value;
                OnPropertyChanged();
            }
        }

        private void ReRunPlotsTimeXAxis_AxisChanged1(object sender, AxisChangedEventArgs e)
        {
            var xAxis = sender as OxyPlot.Axes.DateTimeAxis;
            var parent = xAxis.Parent;
            foreach (var dtr in MMReRunPlotModels)
            {
                if (dtr.MMReRunAllSignalsPlotModel != parent)
                {
                    foreach (var ax in dtr.MMReRunAllSignalsPlotModel.Axes)
                    {
                        if (ax.IsHorizontal() && ax.ActualMinimum != xAxis.ActualMinimum)
                        {
                            ax.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                            break;
                        }
                    }
                dtr.MMReRunAllSignalsPlotModel.InvalidatePlot(false);
                }                
            }
        }
    }
}
