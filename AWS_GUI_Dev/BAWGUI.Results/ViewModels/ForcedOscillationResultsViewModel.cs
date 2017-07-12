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
                //_models = value;
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
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(_selectedEndTime))
                {
                    _filterTableByTime();
                }
                OnPropertyChanged();
            }
        }

        private string _selectedEndTime;
        public string SelectedEndTime
        {
            get { return _selectedEndTime; }
            set
            {
                _selectedEndTime = value;
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(_selectedStartTime))
                {
                    _filterTableByTime();
                }
                OnPropertyChanged();
            }
        }

        private void _filterTableByTime()
        {
            ObservableCollection<ForcedOscillationResultViewModel> newResults = new ObservableCollection<ForcedOscillationResultViewModel>();
            DateTime startT = DateTime.Parse(SelectedStartTime);
            DateTime endT = DateTime.Parse(SelectedEndTime);
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
                        evnt.FilteredOccurrences = newOcurs;
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
            FilteredResults = newResults;
            _drawFOPlot();
        }

        private ForcedOscillationResultViewModel _selectedOscillationEvent;
        public ForcedOscillationResultViewModel SelectedOscillationEvent
        {
            get { return _selectedOscillationEvent; }
            set
            {
                _selectedOscillationEvent = value;
                OnPropertyChanged();
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
            }
        }

        private OccurrenceTableWindow _occurrenceTableWin;
        public ICommand ShowOccurrenceWindow { get; set; }
        private void _showOccurrenceWindow(object obj)
        {
            bool isWindowOpen = false;
            foreach(var w in Application.Current.Windows)
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
                _occurrenceTableWin.Owner = Application.Current.MainWindow;
                _occurrenceTableWin.Show();
            }
        }

        private ChannelTableWindow _channelTableWin;
        public ICommand ShowChannelWindow { get; set; }
        private void _showChannelWindow(object obj)
        {
            bool isWindowOpen = false;
            foreach (var w in Application.Current.Windows)
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
            var startTime = FilteredResults.Min(x => x.GetFirstStartOfFilteredOccurrences());
            var endTime = FilteredResults.Max(x => x.GetLastEndOfFilteredOccurrences());
            var time = Convert.ToDateTime(endTime) - Convert.ToDateTime(startTime);
            if(time < TimeSpan.FromHours(24))
            {
                xAxisFormatString = "HH:mm";
            }
            else if(time >= TimeSpan.FromHours(24) && time < TimeSpan.FromHours(168))
            {
                xAxisFormatString = "MM/dd HH";
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
                Title = "Time",
                IsZoomEnabled = true,
                IsPanEnabled = true,
                StringFormat = xAxisFormatString
            };
            //a.Axes.Add(timeXAxis);

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
            if (FilteredResults.Count > 0)
            {
                frequencyYAxis.Maximum = FilteredResults.Select(x => x.TypicalFrequency).Max() + 0.1;
                frequencyYAxis.Minimum = FilteredResults.Select(x => x.TypicalFrequency).Min() - 0.1;
            }
            a.Axes.Add(frequencyYAxis);
            //var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };
            //a.DefaultColors = OxyPalettes.BlueWhiteRed(FilteredResults.Count).Colors;
            //int index = 0;
            var alarmSeries = new ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 4};
            foreach (var fo in FilteredResults)
            {
                //OxyColor eventColor = a.DefaultColors[index];
                OxyColor eventColor = _mapFrequencyToColor(fo.TypicalFrequency);
                foreach (var ocur in fo.FilteredOccurrences)
                {
                    //var newSeries = new LineSeries { Title = fo.Label };
                    var newSeries = new LineSeries() { LineStyle= LineStyle.Solid, Color = eventColor, StrokeThickness = 5};
                    newSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start)), ocur.Frequency));
                    newSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End)), ocur.Frequency));
                    a.Series.Add(newSeries);
                    if (ocur.Alarm == "YES")
                    {
                        var startPoint = new ScatterPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start)), ocur.Frequency);
                        var endPoint = new ScatterPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End)), ocur.Frequency);
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
            a.Axes.Add(timeXAxis);

            FOPlotModel = a;
        }

        private OxyColor _mapFrequencyToColor(float frequency)
        {
            OxyColor color;
            var colorCount = FilteredResults.Count;
            var minFreq = FilteredResults.Select(x => x.TypicalFrequency).Min();
            var maxFreq = FilteredResults.Select(x => x.TypicalFrequency).Max();
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
    }
}
