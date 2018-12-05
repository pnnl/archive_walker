using BAWGUI.Core;
using BAWGUI.MATLABRunResults.Models;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Utilities;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                _run = value;
                OnPropertyChanged();
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
            throw new NotImplementedException();
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
                var latest = allName.Max();
                SelectedStartTime = latest.ToString("MM/dd/yyyy HH:mm:ss");
                SelectedEndTime = latest.AddDays(1).AddSeconds(-1).ToString("MM/dd/yyyy HH:mm:ss");
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
                if (Path.GetExtension(file).ToLower() == "csv")
                {
                    try
                    {
                        var filename = DateTime.ParseExact(Path.GetFileNameWithoutExtension(file), "yyMMdd", CultureInfo.InvariantCulture);
                        allName.Add(filename);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        }

        //private List<DateTime> _findAllCSVDataFileNames(string path)
        //{
        //    foreach (var dir in Directory.GetDirectories(path))
        //    {

        //    }
        //    foreach (var file in Directory.GetFiles(path))
        //    {
        //        if (Path.GetExtension(file).ToLower() == "csv")
        //        {
        //            var filename = DateTime.ParseExact(Path.GetFileNameWithoutExtension(file), "yyMMdd", CultureInfo.InvariantCulture);
        //        }
        //    }
        //    return new List<DateTime>();
        //}
    }
}
