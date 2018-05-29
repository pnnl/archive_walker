using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Xml;
using System.Windows;
using BAWGUI.Results.Models;
using System.Windows.Input;
using BAWGUI.Results.Views;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.RunMATLAB.Models;
using System.Windows.Forms;

namespace BAWGUI.Results.ViewModels
{
    /// <summary>
    /// The ViewModel for the full Forced Oscillation results view.
    /// </summary>
    public class ForcedOscillationResultsViewModel : ViewModelBase
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //private ForcedOscillationType[] _models;
        public ForcedOscillationResultsViewModel()
        {
            ShowOccurrenceWindow = new RelayCommand(_showOccurrenceWindow);
            ShowChannelWindow = new RelayCommand(_showChannelWindow);
            //ShowOccurrencePopup = new RelayCommand(_showOccurrencePopup);
            //CloseOccurrencePopup = new RelayCommand(_closeOccurrencePopup);
            _results = new ObservableCollection<ForcedOscillationResultViewModel>();
            _filteredResults = new ObservableCollection<ForcedOscillationResultViewModel>();
            _models = new List<DatedForcedOscillationEvent>();
            _foPlotModel = new PlotModel();
            _selectedOscillationEvent = new ForcedOscillationResultViewModel();
            _selectedOccurrence = new OccurrenceViewModel();
            _configFilePath = "";
            _run = new AWRunViewModel();
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
        //private ObservableCollection<ForcedOscillationResultViewModel> _results = new ObservableCollection<ForcedOscillationResultViewModel>();
        //private ObservableCollection<ForcedOscillationResultViewModel> _filteredResults = new ObservableCollection<ForcedOscillationResultViewModel>();
        private ObservableCollection<ForcedOscillationResultViewModel> _results;
        private ObservableCollection<ForcedOscillationResultViewModel> _filteredResults;
        public ObservableCollection<ForcedOscillationResultViewModel> FilteredResults
        {
            get { return _filteredResults; }
            set
            {
                _filteredResults = value;
                OnPropertyChanged();
            }
        }
        //private List<DatedForcedOscillationEvent> _models = new List<DatedForcedOscillationEvent>();
        private List<DatedForcedOscillationEvent> _models;
        public List<DatedForcedOscillationEvent> Models
        {
            get { return this._models; }
            set
            {
                _models = value;
                _results.Clear();
                _filteredResults.Clear();
                foreach (var model in value)
                {
                    _results.Add(new ForcedOscillationResultViewModel(model));
                    _filteredResults.Add(new ForcedOscillationResultViewModel(model));


                    ////flattened occurrence with events
                    //foreach (var ocur in model.Occurrence)
                    //{
                    //    _results.Add(new ForcedOscillationResultViewModel(model, ocur));
                    //    _filteredResults.Add(new ForcedOscillationResultViewModel(model, ocur));
                    //}
                }
                //_filteredResults.OrderBy(x => x.TypicalFrequency).OrderBy(y => y.Alarm);
                //_filteredResults = new ObservableCollection<ForcedOscillationResultViewModel>(_filteredResults.OrderByDescending(x => x.OverallStartTime).OrderByDescending(y => y.Alarm));
                // We shouldn't need this thanks to the ObservableCollection.
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
                    //System.Windows.Forms.MessageBox.Show("Selected start time is later than end time.", "Error!", MessageBoxButtons.OK);
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
                        //System.Windows.Forms.MessageBox.Show("Selected end time is earlier than start time.", "Error!", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void _filterTableByTime()
        {
            ObservableCollection<ForcedOscillationResultViewModel> newResults = new ObservableCollection<ForcedOscillationResultViewModel>();
            DateTime startT = DateTime.Parse(_selectedStartTime);
            DateTime endT = DateTime.Parse(_selectedEndTime);
            foreach (var evnt in _results)
            {
                DateTime st = DateTime.Parse(evnt.OverallStartTime);
                DateTime ed = DateTime.Parse(evnt.OverallEndTime);
                if (DateTime.Compare(st, endT) <= 0 && DateTime.Compare(ed, startT) >= 0)
                {
                    ObservableCollection<OccurrenceViewModel> newOcurs = new ObservableCollection<OccurrenceViewModel>();
                    foreach (var ocur in evnt.Occurrences)
                    {
                        DateTime ocurst = DateTime.Parse(ocur.Start);
                        DateTime ocured = DateTime.Parse(ocur.End);
                        if (DateTime.Compare(ocurst, endT) <= 0 && DateTime.Compare(ocured, startT) >= 0)
                        {
                            newOcurs.Add(ocur);
                        }
                    }
                    if (newOcurs.Count != 0)
                    {
                        evnt.FilteredOccurrences = new ObservableCollection<OccurrenceViewModel>(newOcurs.OrderBy(x => x.Start).OrderByDescending(y => y.Alarm));
                        newResults.Add(evnt);
                    }

                    ////flattened occurence with events
                    //DateTime ocurst = DateTime.Parse(evnt.Occurrence.Start);
                    //DateTime ocured = DateTime.Parse(evnt.Occurrence.End);
                    //if (DateTime.Compare(ocurst, endT) <= 0 && DateTime.Compare(ocured, startT) >= 0)
                    //{
                    //    newResults.Add(evnt);
                    //}
                }
            }
            FilteredResults = new ObservableCollection<ForcedOscillationResultViewModel>(newResults.OrderBy(x => x.OverallStartTime).OrderByDescending(y => y.Alarm));
            if (FilteredResults.Count() != 0)
            {
                _drawFOPlot();
            }
            //else
            //{
            //    MessageBox.Show("Selected time range has no results!", "Warning", MessageBoxButton.OK);
            //}
        }

        private ForcedOscillationResultViewModel _selectedOscillationEvent;
        public ForcedOscillationResultViewModel SelectedOscillationEvent
        {
            get { return _selectedOscillationEvent; }
            set
            {
                _selectedOscillationEvent = value;
                OnPropertyChanged();
                if (_selectedOscillationEvent != null)
                {
                    bool ocurFound = false;
                    foreach (var ocur in _selectedOscillationEvent.FilteredOccurrences)
                    {
                        if (ocur == _selectedOccurrence)
                        {
                            ocurFound = true;
                            break;
                        }
                    }
                    if (!ocurFound)
                    {
                        SelectedOccurrence = _selectedOscillationEvent.FilteredOccurrences.First();
                    }
                }
            }
        }

        private OccurrenceViewModel _selectedOccurrence;
        public OccurrenceViewModel SelectedOccurrence
        {
            get { return _selectedOccurrence; }
            set
            {
                _selectedOccurrence = value;
                OnPropertyChanged();
                if (_selectedOccurrence != null)
                {
                    foreach (var item in FOPlotModel.Series)
                    {
                        if (item is LineSeries)
                        {
                            var it = item as LineSeries;
                            if (it.StrokeThickness == 10)
                            {
                                it.StrokeThickness = 5;
                            }
                            if (it.Points[0].X == DateTimeAxis.ToDouble(Convert.ToDateTime(_selectedOccurrence.Start)) && it.Points[0].Y == _selectedOccurrence.Frequency && it.Points[1].X == DateTimeAxis.ToDouble(Convert.ToDateTime(_selectedOccurrence.End)))
                            {
                                it.StrokeThickness = 10;
                                FOPlotModel.InvalidatePlot(true);
                            }
                        }
                    }
                }
            }
        }

        private OccurrenceTableWindow _occurrenceTableWin;
        public ICommand ShowOccurrenceWindow { get; set; }
        private void _showOccurrenceWindow(object obj)
        {
            bool isWindowOpen = false;
            foreach(var w in System.Windows.Application.Current.Windows)
            {
                if( w is OccurrenceTableWindow)
                {
                    isWindowOpen = true;
                    ((OccurrenceTableWindow)w).DataContext = this;
                    ((OccurrenceTableWindow)w).Activate();
                }
            }
            if(!isWindowOpen)
            {
                _occurrenceTableWin = new OccurrenceTableWindow();
                _occurrenceTableWin.DataContext = this;
                _occurrenceTableWin.Owner = System.Windows.Application.Current.MainWindow;
                _occurrenceTableWin.Show();
            }
        }

        private ChannelTableWindow _channelTableWin;
        public ICommand ShowChannelWindow { get; set; }
        private void _showChannelWindow(object obj)
        {
            bool isWindowOpen = false;
            foreach (var w in System.Windows.Application.Current.Windows)
            {
                if (w is ChannelTableWindow)
                {
                    isWindowOpen = true;
                    ((ChannelTableWindow)w).DataContext = this;
                    ((ChannelTableWindow)w).Activate();
                }
            }
            if (!isWindowOpen)
            {
                _channelTableWin = new ChannelTableWindow();
                _channelTableWin.DataContext = this;
                _channelTableWin.Owner = _occurrenceTableWin;
                _channelTableWin.Show();
            }
        }
        //private bool _isOccurrencePopupOpen;
        //public bool IsOccurrencePopupOpen
        //{
        //    get { return _isOccurrencePopupOpen; }
        //    set
        //    {
        //        _isOccurrencePopupOpen = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public ICommand ShowOccurrencePopup { get; set; }
        //private void _showOccurrencePopup(object obj)
        //{
        //    IsOccurrencePopupOpen = true;
        //}
        //public ICommand CloseOccurrencePopup { get; set; }
        //private void _closeOccurrencePopup(object obj)
        //{
        //    IsOccurrencePopupOpen = false;
        //}

        private PlotModel _foPlotModel;
        public PlotModel FOPlotModel
        {
            get { return _foPlotModel; }
            set
            {
                _foPlotModel = value;
                OnPropertyChanged();
            }
        }

        private void _drawFOPlot()
        {
            PlotModel a = new PlotModel() { PlotAreaBackground = OxyColors.LightGray };
            //{ PlotAreaBackground = OxyColors.WhiteSmoke}
            //a.PlotType = PlotType.Cartesian;
            var xAxisFormatString = "";
            var startTime = new DateTime();
            var endTime = new DateTime();
            if (FilteredResults.Count > 0)
            {
                startTime = Convert.ToDateTime(FilteredResults.Min(x => x.GetFirstStartOfFilteredOccurrences()));
                endTime = Convert.ToDateTime(FilteredResults.Max(x => x.GetLastEndOfFilteredOccurrences()));
            }
            var time = Convert.ToDateTime(endTime) - Convert.ToDateTime(startTime);
            if(time < TimeSpan.FromHours(24))
            {
                xAxisFormatString = "HH:mm";
            }
            else if(time >= TimeSpan.FromHours(24) && time < TimeSpan.FromHours(168))
            {
                xAxisFormatString = "MM/dd\nHH:mm";
            }
            else
            {
                xAxisFormatString = "MM/dd";
            }
            DateTimeAxis timeXAxis = new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                MinorIntervalType = DateTimeIntervalType.Auto,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                //Title = "Time",
                IsZoomEnabled = true,
                IsPanEnabled = true,
                StringFormat = xAxisFormatString,
            };
            //a.Axes.Add(timeXAxis);
            timeXAxis.AxisChanged += TimeXAxis_AxisChanged;
            LinearAxis frequencyYAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Frequency (Hz)",
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            frequencyYAxis.AxisChanged += FrequencyYAxis_AxisChanged;

            float axisMax = FilteredResults.Select(x => x.TypicalFrequency).Max() + (float)0.1;
            float axisMin = FilteredResults.Select(x => x.TypicalFrequency).Min() - (float)0.1;
            if (FilteredResults.Count > 0)
            {
                frequencyYAxis.Maximum = axisMax;
                frequencyYAxis.Minimum = axisMin;
            }
            a.Axes.Add(frequencyYAxis);
            //var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };
            //a.DefaultColors = OxyPalettes.BlueWhiteRed(FilteredResults.Count).Colors;
            //int index = 0;
            a.DefaultColors.Clear();
            var alarmSeries = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 4, Title = "Alarms", ColorAxisKey = null};
            //var heatMapData = new List<float>();
            var trackerKey = 0;
            foreach (var fo in FilteredResults)
            {
                //OxyColor eventColor = a.DefaultColors[index];
                OxyColor eventColor = _mapFrequencyToColor(fo.TypicalFrequency);
                a.DefaultColors.Add(eventColor);
                //heatMapData.Add(fo.TypicalFrequency);
                foreach (var ocur in fo.FilteredOccurrences)
                {
                    //var newSeries = new LineSeries { Title = fo.Label };
                    var newSeries = new LineSeries() { LineStyle= LineStyle.Solid, Color = eventColor, StrokeThickness = 5};
                    if (ocur == SelectedOccurrence)
                    {
                        newSeries.StrokeThickness = 10;
                    }
                    newSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start)), ocur.Frequency));
                    newSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End)), ocur.Frequency));
                    a.Series.Add(newSeries);
                    newSeries.TrackerKey = trackerKey.ToString();
                    ocur.trackerKey = trackerKey;
                    trackerKey++;
                    newSeries.MouseDown += foEvent_MouseDown;
                    if (ocur.Alarm == "YES")
                    {
                        var startPoint = new ScatterPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start)), ocur.Frequency, 4, 0);
                        var endPoint = new ScatterPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End)), ocur.Frequency, 4, 0);
                        //aPoint.Size = 5;
                        //aPoint.Tag = "Alarm";
                        alarmSeries.Points.Add(startPoint);
                        alarmSeries.Points.Add(endPoint);
                    }
                }
                //index++;
                //categoryAxis.Labels.Add(fo.TypicalFrequency.ToString());
            }
            a.Series.Add(alarmSeries);

            //heatMapData.Sort();
            if (a.DefaultColors.Count > 0)
            {
                a.DefaultColors.Add(_mapFrequencyToColor(axisMax));
                a.DefaultColors.Add(_mapFrequencyToColor(axisMin));
                a.Axes.Add(new LinearColorAxis { Palette = new OxyPalette(a.DefaultColors.OrderBy(x => x.G)), Position = AxisPosition.Right, Minimum = axisMin, Maximum = axisMax, Title = "Frequency (Hz)", MajorStep = 0.2});
            }
            //else
            //    a.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Minimum = heatMapData.Min(), Maximum = heatMapData.Max(), Title = "Frequency (Hz)", MajorStep = 0.2 });
            //var frequencyHeatMap = new HeatMapSeries
            //{
            //    X0 = 0,
            //    X1 = 0.1,
            //    Y0 = heatMapData.Min(),
            //    Y1 = heatMapData.Max(),
            //    RenderMethod = HeatMapRenderMethod.Bitmap,
            //    Interpolate = true,
            //    //Data = heatMapData.ToArray()
            //};
            //a.Series.Add(frequencyHeatMap);

            a.Axes.Add(timeXAxis);
            a.LegendPlacement = LegendPlacement.Outside;
            a.LegendPosition = LegendPosition.TopRight;
            //a.LegendOrientation = LegendOrientation.Horizontal;
            a.LegendPadding = 0.0;
            a.LegendSymbolMargin = 0.0;
            a.LegendMargin = 0;
            //a.LegendSymbolPlacement = LegendSymbolPlacement.Right;
            var currentArea = a.LegendArea;
            var currentPlotWithAxis = a.PlotAndAxisArea;

                //= new OxyRect(currentArea.Left, currentArea.Top - 10, currentArea.Width, currentArea.Height);

            var currentMargins = a.PlotMargins;
            a.PlotMargins = new OxyThickness(currentMargins.Left, currentMargins.Top, 80, currentMargins.Bottom);
            FOPlotModel = a;
        }

        private void foEvent_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var s = (LineSeries)sender;
            //var old = Queryable.Where<Series>(FOPlotModel.Series.AsQueryable(), p => p.GetType() is typeof(LineSeries) && p.StrokeThickness==10); //FOPlotModel.Series.Where((LineSeries x) => x == s);
            //foreach (var item in FOPlotModel.Series)
            //{
            //    if(item is LineSeries)
            //    {
            //        var it = item as LineSeries;
            //        if(it.StrokeThickness == 10)
            //        {
            //            it.StrokeThickness = 5;
            //        }
            //    }
            //}
            //old.FirstOrDefault();
            //var x = (sender as LineSeries).InverseTransform(e.Position).X;
            //var y = (sender as LineSeries).InverseTransform(e.Position).Y;
            //s.StrokeThickness = 10;
            bool ocurFound = false;
            foreach (var fo in FilteredResults)
            {
                foreach (var ocur in fo.FilteredOccurrences)
                {
                    if(ocur.trackerKey.ToString() == s.TrackerKey)
                    {
                        ocurFound = true;
                        SelectedOscillationEvent = fo;
                        SelectedOccurrence = ocur;
                        break;
                    }
                }
                if (ocurFound)
                {
                    break;
                }
            }
            //if (ocurFound)
            //{
            //    FOPlotModel.InvalidatePlot(true);
            //}
             //MessageBox.Show(DateTimeAxis.ToDateTime(x).ToString());
        }

        private void FrequencyYAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            var fAxis = sender as LinearAxis;
            Console.WriteLine("frequency axis changed! do stuff!" + fAxis.ActualMaximum.ToString() + ", " + fAxis.ActualMinimum.ToString());
        }

        private void TimeXAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            var xAxis = sender as DateTimeAxis;
            Console.WriteLine("x axis changed! do stuff!" + xAxis.ActualMaximum.ToString() + ", " + xAxis.ActualMinimum.ToString());
        }

        private OxyColor _mapFrequencyToColor(float frequency)
        {
            //OxyColor color;
            var colorCount = FilteredResults.Count;
            var minFreq = FilteredResults.Select(x => x.TypicalFrequency).Min() - 0.1;
            var maxFreq = FilteredResults.Select(x => x.TypicalFrequency).Max() + 0.1;
            var percentage = (frequency - minFreq) / (maxFreq - minFreq);

            //blue-green rgb gradient
            return OxyColor.FromRgb(0, Convert.ToByte(255 * percentage), Convert.ToByte(255 * (1 - percentage)));

            //blue-purple-red gradient
            //return OxyColor.FromRgb(Convert.ToByte(255 * percentage), 0, Convert.ToByte(255 * (1 - percentage)));

            //blue-white-red gradient
            //if (percentage < 0.5)
            //{
            //    return OxyColor.FromRgb(Convert.ToByte(255 * percentage), Convert.ToByte(255 * percentage), 255);
            //}
            //else
            //{
            //    return OxyColor.FromRgb(255, Convert.ToByte(255 * (1 - percentage)), Convert.ToByte(255 * (1 - percentage)));
            //}

            //blue-white-green gradient
            //if (percentage < 0.5)
            //{
            //    return OxyColor.FromRgb(Convert.ToByte(255 * percentage), Convert.ToByte(255 * percentage), 255);
            //}
            //else
            //{
            //    return OxyColor.FromRgb(Convert.ToByte(255 * (1 - percentage)), 255, Convert.ToByte(255 * (1 - percentage)));
            //}
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
    }
}
