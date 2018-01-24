using BAWGUI.Results.Models;
using BAWGUI.RunMATLAB.Models;
using BAWGUI.RunMATLAB.ViewModels;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class RingdownResultsViewModel:ViewModelBase
    {
        public RingdownResultsViewModel()
        {
            _results = new ObservableCollection<RingdownEventViewModel>();
            _filteredResults = new ObservableCollection<RingdownEventViewModel>();
            _models = new List<RingDownEvent>();
            _sparseResults = new List<SparseResult>();
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
            FilteredResults = new ObservableCollection<RingdownEventViewModel>(newResults.OrderBy(x => x.StartTime));
            if (SparseResults.Count() != 0)
            {
            }
        }
        private RingdownEventViewModel _selectedRingdownEvent;
        public RingdownEventViewModel SelectedRingdownEvent
        {
            get { return _selectedRingdownEvent; }
            set
            {
                _selectedRingdownEvent = value;
                OnPropertyChanged();
            }
        }

        private List<SparseResult> _sparseResults;
        public List<SparseResult> SparseResults
        {
            get { return _sparseResults; }
            set
            {
                _sparseResults = value;
                if (SparseResults.Count() != 0)
                {
                    _drawRDPlot();
                }
                OnPropertyChanged();
            }
        }
        private PlotModel _rdPlotModel;
        public PlotModel RDPlotModel
        {
            get { return _rdPlotModel; }
            set
            {
                _rdPlotModel = value;
                OnPropertyChanged();
            }
        }

        private void _drawRDPlot()
        {
            PlotModel a = new PlotModel() { PlotAreaBackground = OxyColors.LightGray };
            //{ PlotAreaBackground = OxyColors.WhiteSmoke}
            //a.PlotType = PlotType.Cartesian;
            var xAxisFormatString = "";
            //var startTime = SparseResults.Min(x => x.TimeStamps.FirstOrDefault());
            //var endTime = SparseResults.Max(x => x.TimeStamps.LastOrDefault());
            //var time = Convert.ToDateTime(endTime) - Convert.ToDateTime(startTime);
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
                //Title = "Time",
                IsZoomEnabled = true,
                IsPanEnabled = true,
                StringFormat = xAxisFormatString,
            };
            //a.Axes.Add(timeXAxis);
            //timeXAxis.AxisChanged += TimeXAxis_AxisChanged;
            OxyPlot.Axes.LinearAxis frequencyYAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Frequency (Hz)",
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            //frequencyYAxis.AxisChanged += FrequencyYAxis_AxisChanged;

            //float axisMax = SparseResults.Select(x => x.TypicalFrequency).Max() + (float)0.1;
            //float axisMin = SparseResults.Select(x => x.TypicalFrequency).Min() - (float)0.1;
            //if (SparseResults.Count > 0)
            //{
            //    frequencyYAxis.Maximum = axisMax;
            //    frequencyYAxis.Minimum = axisMin;
            //}
            //a.Axes.Add(frequencyYAxis);
            //var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };
            //a.DefaultColors = OxyPalettes.BlueWhiteRed(FilteredResults.Count).Colors;
            //int index = 0;
            //a.DefaultColors.Clear();
            //var alarmSeries = new OxyPlot.Series.ScatterSeries() { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 4, Title = "Alarms", ColorAxisKey = null };
            //var trackerKey = 0;
            foreach (var rd in SparseResults)
            {
                //OxyColor eventColor = _mapFrequencyToColor(fo.TypicalFrequency);
                //a.DefaultColors.Add(eventColor);
                //foreach (var ocur in fo.FilteredOccurrences)
                //{
                    //var newSeries = new LineSeries { Title = fo.Label };
                var newSeries = new OxyPlot.Series.AreaSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 5 };
                //if (ocur == SelectedOccurrence)
                //{
                //    newSeries.StrokeThickness = 10;
                //}
                //newSeries.Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start)), ocur.Frequency));
                //newSeries.Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End)), ocur.Frequency));
                //a.Series.Add(newSeries);
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

            a.Axes.Add(timeXAxis);
            a.LegendPlacement = LegendPlacement.Outside;
            a.LegendPosition = LegendPosition.TopRight;
            a.LegendPadding = 0.0;
            a.LegendSymbolMargin = 0.0;
            a.LegendMargin = 0;
            var currentArea = a.LegendArea;
            var currentPlotWithAxis = a.PlotAndAxisArea;

            var currentMargins = a.PlotMargins;
            a.PlotMargins = new OxyThickness(currentMargins.Left, currentMargins.Top, 80, currentMargins.Bottom);
            RDPlotModel = a;

        }
    }
}
