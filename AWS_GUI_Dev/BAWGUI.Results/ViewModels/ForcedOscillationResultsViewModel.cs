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
using System.Windows.Forms;
using BAWGUI.Core;
using BAWGUI.Utilities;
using BAWGUI.MATLABRunResults.Models;
using MapService.ViewModels;
using BAWGUI.SignalManagement.ViewModels;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Media.Animation;
using BAWGUI.CoordinateMapping.ViewModels;
using BAWGUI.Core.Models;

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
            //_selectedOccurrence = new OccurrenceViewModel();
            _configFilePath = "";
            _run = new AWRunViewModel();
            _signalMgr = SignalManager.Instance;
            ResultMapVM = new ResultMapViewModel();
            //_selectedStartTime = "01/01/0001 00:00:00";
            //_selectedEndTime = "01/01/0001 00:00:00";
            _selectedStartTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            _selectedEndTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            _mapPlottingRule = new List<string>(new string[]{ "SNR", "Amplitude", "Coherence", "DEF" });
            _selectedPlottingRule = "SNR";
            Areas = new List<EnergyFlowAreaCoordsMappingViewModel>();
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
                    var mm = new ForcedOscillationResultViewModel(model);
                    mm.SelectedOccurrenceChanged += _selectedOccurrenceChanged;
                    mm.SelectedChannelChanged += _selectedChannelChanged;
                    _results.Add(mm);
                    _filteredResults.Add(mm);


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
                //SelectedOscillationEvent = FilteredResults.FirstOrDefault();
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
                //var startTime = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                //var endTime = Convert.ToDateTime(_selectedEndTime, CultureInfo.InvariantCulture);
                var startTime = DateTime.Now;
                var endTime = DateTime.Now;
                try
                {
                    startTime = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("In fo results viewmodel, SelectedStartTime, startTime Convert;");
                }
                try
                {
                    endTime = Convert.ToDateTime(_selectedEndTime, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("In fo results viewmodel, SelectedStartTime, endTime Convert;");
                }
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
                    //var startTime = Convert.ToDateTime(_selectedStartTime, CultureInfo.InvariantCulture);
                    //var endTime = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                    var startTime = DateTime.Now;
                    var endTime = DateTime.Now;
                    try
                    {
                        startTime = Convert.ToDateTime(_selectedStartTime, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        System.Windows.Forms.MessageBox.Show("In fo results viewmodel, SelectedEndTime, startTime Convert;");
                    }
                    try
                    {
                        endTime = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        System.Windows.Forms.MessageBox.Show("In fo results viewmodel, SelectedEndTime, endTime Convert;");
                    }
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
            DateTime startT = DateTime.Parse(_selectedStartTime, CultureInfo.InvariantCulture);
            DateTime endT = DateTime.Parse(_selectedEndTime, CultureInfo.InvariantCulture);
            foreach (var evnt in _results)
            {
                DateTime st = DateTime.Parse(evnt.OverallStartTime, CultureInfo.InvariantCulture);
                DateTime ed = DateTime.Parse(evnt.OverallEndTime, CultureInfo.InvariantCulture);
                if (DateTime.Compare(st, endT) <= 0 && DateTime.Compare(ed, startT) >= 0)
                {
                    ObservableCollection<OccurrenceViewModel> newOcurs = new ObservableCollection<OccurrenceViewModel>();
                    foreach (var ocur in evnt.Occurrences)
                    {
                        DateTime ocurst = DateTime.Parse(ocur.Start, CultureInfo.InvariantCulture);
                        DateTime ocured = DateTime.Parse(ocur.End, CultureInfo.InvariantCulture);
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
                    //DateTime ocurst = DateTime.Parse(evnt.Occurrence.Start, CultureInfo.InvariantCulture);
                    //DateTime ocured = DateTime.Parse(evnt.Occurrence.End, CultureInfo.InvariantCulture);
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
                _updateSelectionsInTables(SelectedPlottingRule);
                _updateFOMapAfterSelectionChange();
                if (SelectedPlottingRule == "DEF")
                {
                    //draw energy flow on map with currently selected event and occurrance
                    _drawDEF();
                }
                else
                {
                    _updateFOplotAfterSelectionChange();
                    _updateMapMarkerAfterChannelSelectionChange();
                }
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
                if (_selectedOscillationEvent != value)
                {
                    //save the previous value of selected event and its selected occurrence
                    //once the GUI updates, the selected occurrence is set to null due to the change of the itemssource of the datagrid
                    //this way we can set the selected occurrence back to its original value.
                    OccurrenceViewModel oldSelectedOccurrence = null;
                    ForcedOscillationResultViewModel oldSelectedOscillationEvent = null;
                    if (_selectedOscillationEvent != null)
                    {
                        oldSelectedOccurrence = _selectedOscillationEvent.SelectedOccurrence;
                        oldSelectedOscillationEvent = _selectedOscillationEvent;
                    }
                    _selectedOscillationEvent = value;
                    _updateFOplotAfterSelectionChange();
                    if (SelectedPlottingRule == "DEF")
                    {
                        //draw energy flow with currently selected event and occurence
                        _drawDEF();
                    }
                    else
                    {
                        _updateFOMapAfterSelectionChange();
                    }
                    OnPropertyChanged();
                    if (oldSelectedOscillationEvent != null)
                    {
                        oldSelectedOscillationEvent.SelectedOccurrence = oldSelectedOccurrence;
                    }
                }
                //if (_selectedOscillationEvent != null)
                //{
                //    bool ocurFound = false;
                //    foreach (var ocur in _selectedOscillationEvent.FilteredOccurrences)
                //    {
                //        if (ocur == _selectedOccurrence)
                //        {
                //            ocurFound = true;
                //            break;
                //        }
                //    }
                //    if (!ocurFound)
                //    {
                //        SelectedOccurrence = _selectedOscillationEvent.FilteredOccurrences.First();
                //    }
                //}
            }
        }
        private void _selectedOccurrenceChanged(object sender, EventArgs e)
        {
            //when the oscillation event that raise the selected occurrence changed event is not the SelectedOscillationEvent, we don't need to update the plots
            var orig = sender as ForcedOscillationResultViewModel;
            if (SelectedOscillationEvent == orig)
            {
                _updateFOplotAfterSelectionChange();
                if (SelectedPlottingRule == "DEF")
                {
                    //draw energy flow with currently selected occurrence and event
                    _drawDEF();
                }
                else
                {
                    _updateFOMapAfterSelectionChange();
                }
            }
        }

        private void _drawDEF()
        {
            var entries = new Dictionary<string, Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>>();
            foreach (var area in Areas)
            {
                var key = area.AreaName;
                var type = area.Type;
                var locations = area.Locations;
                entries[key] = new Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>(type, locations.ToList());
            }

            if (entries.Count() != 0 && SelectedOscillationEvent.SelectedOccurrence.Model.Paths.Count() != 0)
            {
                ResultMapVM.DrawDEF(entries, SelectedOscillationEvent.SelectedOccurrence.Model.Paths);
            }
        }

        private void _selectedChannelChanged(object sender, EventArgs e)
        {
            _updateMapMarkerAfterChannelSelectionChange();
        }
        private void _updateMapMarkerAfterChannelSelectionChange()
        {
            if (SelectedOscillationEvent != null && SelectedOscillationEvent.SelectedOccurrence != null && SelectedOscillationEvent.SelectedOccurrence.SelectedChannel != null)
            {
                foreach (var mkr in ResultMapVM.Gmap.Markers)
                {
                    if (mkr.Tag.ToString() == SelectedOscillationEvent.SelectedOccurrence.SelectedChannel.Name)
                    {
                        if (mkr.Shape is Path)
                        {
                            var shape = mkr.Shape as Path;
                            shape.StrokeThickness = 8;
                        }
                        if (mkr.Shape is Ellipse)
                        {
                            var shape = mkr.Shape as Ellipse;
                            shape.Width = 30;
                            shape.Height = 30;
                        }
                    }
                    else
                    {
                        if (mkr.Shape is Path)
                        {
                            var shape = mkr.Shape as Path;
                            shape.StrokeThickness = 4;
                        }
                        if (mkr.Shape is Ellipse)
                        {
                            var shape = mkr.Shape as Ellipse;
                            shape.Width = 15;
                            shape.Height = 15;
                        }
                    }
                }
            }
        }
        private void _updateFOMapAfterSelectionChange()
        {
            ResultMapVM.ClearMarkers();
            if (SelectedOscillationEvent != null && SelectedOscillationEvent.SelectedOccurrence != null)
            {
                var signalList = new List<SignalIntensityViewModel>();
                foreach (var channel in SelectedOscillationEvent.SelectedOccurrence.Channels)
                {
                    var signal = _signalMgr.SearchForSignalInTaggedSignals(channel.PMU, channel.Name);
                    if (signal != null)
                    {
                        switch (SelectedPlottingRule)
                        {
                            case "SNR":
                                signalList.Add(new SignalIntensityViewModel(signal, channel.SNR));
                                break;
                            case "Amplitude":
                                signalList.Add(new SignalIntensityViewModel(signal, channel.Amplitude));
                                break;
                            case "Coherence":
                                signalList.Add(new SignalIntensityViewModel(signal, channel.Coherence));
                                break;
                            case "DEF":

                            default:
                                break;
                        }
                    }
                }
                ////ResultMapVM.Signals = signalList;
                ResultMapVM.UpdateResultMap(signalList);
                //TODO: update map here
                //need to pass more information to the update map function by telling how to distinguish the intensity of the drawings
                //need to know if it's by SNR, Amplitude or Coherence, as selected drawing property.
                //need to add a property in the signalviewmodel, might be called intensity, or add a wrapper class that wraps signalsignatureviewmodel and has intensity property.
            }
        }

        private void _updateFOplotAfterSelectionChange()
        {
            if (SelectedOscillationEvent != null && SelectedOscillationEvent.SelectedOccurrence != null)
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
                        if (it.Points[0].X == DateTimeAxis.ToDouble(Convert.ToDateTime(SelectedOscillationEvent.SelectedOccurrence.Start, CultureInfo.InvariantCulture)) && it.Points[0].Y == SelectedOscillationEvent.SelectedOccurrence.Frequency && it.Points[1].X == DateTimeAxis.ToDouble(Convert.ToDateTime(SelectedOscillationEvent.SelectedOccurrence.End)))
                        {
                            it.StrokeThickness = 10;
                            FOPlotModel.InvalidatePlot(true);
                        }
                    }
                }
            }
            else
            {// change all plots to normal size since nothing is selected in table.
                foreach (var item in FOPlotModel.Series)
                {
                    if (item is LineSeries)
                    {
                        var it = item as LineSeries;
                        if (it.StrokeThickness == 10)
                        {
                            it.StrokeThickness = 5;
                        }
                    }
                }
            }
        }

        //private OccurrenceViewModel _selectedOccurrence;
        //public OccurrenceViewModel SelectedOccurrence
        //{
        //    get { return _selectedOccurrence; }
        //    set
        //    {
        //        if (_selectedOccurrence != value)
        //        {
        //            _selectedOccurrence = value;
        //            OnPropertyChanged();
        //            ResultMapVM.ClearMarkers();
        //            if (_selectedOccurrence != null)
        //            {
        //                foreach (var item in FOPlotModel.Series)
        //                {
        //                    if (item is LineSeries)
        //                    {
        //                        var it = item as LineSeries;
        //                        if (it.StrokeThickness == 10)
        //                        {
        //                            it.StrokeThickness = 5;
        //                        }
        //                        if (it.Points[0].X == DateTimeAxis.ToDouble(Convert.ToDateTime(_selectedOccurrence.Start, CultureInfo.InvariantCulture)) && it.Points[0].Y == _selectedOccurrence.Frequency && it.Points[1].X == DateTimeAxis.ToDouble(Convert.ToDateTime(_selectedOccurrence.End)))
        //                        {
        //                            it.StrokeThickness = 10;
        //                            FOPlotModel.InvalidatePlot(true);
        //                        }
        //                    }
        //                }
        //                var signalList = new List<SignalIntensityViewModel>();
        //                foreach (var channel in _selectedOccurrence.Channels)
        //                {
        //                    var signal = _signalMgr.SearchForSignalInTaggedSignals(channel.PMU, channel.Name);
        //                    if (signal != null)
        //                    {
        //                        switch (SelectedPlottingRule)
        //                        {
        //                            case "SNR":
        //                                signalList.Add(new SignalIntensityViewModel(signal, channel.SNR));
        //                                break;
        //                            case "Amplitude":
        //                                signalList.Add(new SignalIntensityViewModel(signal, channel.Amplitude));
        //                                break;
        //                            case "Coherence":
        //                                signalList.Add(new SignalIntensityViewModel(signal, channel.Coherence));
        //                                break;
        //                            default:
        //                                break;
        //                        }
        //                    }
        //                }
        //                ////ResultMapVM.Signals = signalList;
        //                ResultMapVM.UpdateResultMap(signalList);
        //                //TODO: update map here
        //                //need to pass more information to the update map function by telling how to distinguish the intensity of the drawings
        //                //need to know if it's by SNR, Amplitude or Coherence, as selected drawing property.
        //                //need to add a property in the signalviewmodel, might be called intensity, or add a wrapper class that wraps signalsignatureviewmodel and has intensity property.
        //            }
        //        }
        //    }
        //}
        public List<string> _mapPlottingRule { get; private set; }
        public List<string> MapPlottingRule { get { return _mapPlottingRule; } }
        private string _selectedPlottingRule;
        public string SelectedPlottingRule
        {
            get { return _selectedPlottingRule; }
            set
            {
                _selectedPlottingRule = value;
                _updateSelectionsInTables(value);
                _updateFOplotAfterSelectionChange();
                if (value == "DEF")
                {
                    //draw energy flow on map with currently selected event and occurrence.
                    _drawDEF();
                }
                else
                {
                    _updateFOMapAfterSelectionChange();
                }
                OnPropertyChanged();
            }
        }
        private void _updateSelectionsInTables(string rule)
        {
            switch (rule)
            {
                case "SNR":
                    if (SelectedOscillationEvent != null)
                    {//if an event is selected in the table
                        if (float.IsNaN(SelectedOscillationEvent.MaxSNR))
                        {//the selected event has to have MaxSNR, or the selection will be set to null
                            SelectedOscillationEvent = FilteredResults.Where(x => !float.IsNaN(x.MaxSNR)).FirstOrDefault();
                        }
                        if (SelectedOscillationEvent != null && SelectedOscillationEvent.SelectedOccurrence != null && float.IsNaN(SelectedOscillationEvent.SelectedOccurrence.MaxSNR))
                        {//if both an event and an occurrence are selected, the selected occurrence have to have MaxSNR, or selection will be set to null
                            SelectedOscillationEvent.SelectedOccurrence = SelectedOscillationEvent.FilteredOccurrences.Where(x => !float.IsNaN(x.MaxSNR)).FirstOrDefault();
                        }
                        if (SelectedOscillationEvent != null && SelectedOscillationEvent.SelectedOccurrence != null && SelectedOscillationEvent.SelectedOccurrence.SelectedChannel != null && float.IsNaN(SelectedOscillationEvent.SelectedOccurrence.SelectedChannel.SNR))
                        {//if an event, an occurrence and a channel are selected, the selected channel have to have SNR, or it will be set to null
                            SelectedOscillationEvent.SelectedOccurrence.SelectedChannel = SelectedOscillationEvent.SelectedOccurrence.Channels.Where(x => !float.IsNaN(x.SNR)).FirstOrDefault();
                        }
                    }
                    break;
                case "Amplitude":
                    if (SelectedOscillationEvent != null)
                    {//if an event is selected in the table
                        if (float.IsNaN(SelectedOscillationEvent.MaxAmplitude))
                        {//the selected event has to have MaxAmplitude, or the selection will be set to null
                            SelectedOscillationEvent = FilteredResults.Where(x => !float.IsNaN(x.MaxAmplitude)).FirstOrDefault();
                        }
                        if (SelectedOscillationEvent != null && SelectedOscillationEvent.SelectedOccurrence != null && float.IsNaN(SelectedOscillationEvent.SelectedOccurrence.MaxAmplitude))
                        {//if both an event and an occurrence are selected, the selected occurrence have to have MaxAmplitude, or selection will be set to null
                            SelectedOscillationEvent.SelectedOccurrence = SelectedOscillationEvent.FilteredOccurrences.Where(x => !float.IsNaN(x.MaxAmplitude)).FirstOrDefault();
                        }
                        if (SelectedOscillationEvent != null && SelectedOscillationEvent.SelectedOccurrence != null && SelectedOscillationEvent.SelectedOccurrence.SelectedChannel != null && float.IsNaN(SelectedOscillationEvent.SelectedOccurrence.SelectedChannel.Amplitude))
                        {//if an event, an occurrence and a channel are selected, the selected channel have to have Amplitude, or it will be set to null
                            SelectedOscillationEvent.SelectedOccurrence.SelectedChannel = SelectedOscillationEvent.SelectedOccurrence.Channels.Where(x => !float.IsNaN(x.Amplitude)).FirstOrDefault();
                        }
                    }
                    break;
                case "Coherence":
                    if (SelectedOscillationEvent != null)
                    {//if an event is selected in the table
                        if (float.IsNaN(SelectedOscillationEvent.MaxCoherence))
                        {//the selected event has to have MaxCoherence, or the selection will be set to null
                            SelectedOscillationEvent = FilteredResults.Where(x => !float.IsNaN(x.MaxCoherence)).FirstOrDefault();
                        }
                        if (SelectedOscillationEvent != null && SelectedOscillationEvent.SelectedOccurrence != null && float.IsNaN(SelectedOscillationEvent.SelectedOccurrence.MaxCoherence))
                        {//if both an event and an occurrence are selected, the selected occurrence have to have MaxCoherence, or selection will be set to null
                            SelectedOscillationEvent.SelectedOccurrence = SelectedOscillationEvent.FilteredOccurrences.Where(x => !float.IsNaN(x.MaxCoherence)).FirstOrDefault();
                        }
                        if (SelectedOscillationEvent != null && SelectedOscillationEvent.SelectedOccurrence != null && SelectedOscillationEvent.SelectedOccurrence.SelectedChannel != null && float.IsNaN(SelectedOscillationEvent.SelectedOccurrence.SelectedChannel.Coherence))
                        {//if an event, an occurrence and a channel are selected, the selected channel have to have Coherence, or it will be set to null
                            SelectedOscillationEvent.SelectedOccurrence.SelectedChannel = SelectedOscillationEvent.SelectedOccurrence.Channels.Where(x => !float.IsNaN(x.Coherence)).FirstOrDefault();
                        }
                    }
                    break;
                case "DEF":
                    SelectedOscillationEvent = FilteredResults.FirstOrDefault();
                    break;
                default:
                    break;
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
                startTime = Convert.ToDateTime(FilteredResults.Min(x => x.GetFirstStartOfFilteredOccurrences()), CultureInfo.InvariantCulture);
                endTime = Convert.ToDateTime(FilteredResults.Max(x => x.GetLastEndOfFilteredOccurrences()), CultureInfo.InvariantCulture);
            }
            var time = Convert.ToDateTime(endTime, CultureInfo.InvariantCulture) - Convert.ToDateTime(startTime, CultureInfo.InvariantCulture);
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
                    if (SelectedOscillationEvent != null && ocur == SelectedOscillationEvent.SelectedOccurrence)
                    {
                        newSeries.StrokeThickness = 10;
                    }
                    newSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start, CultureInfo.InvariantCulture)), ocur.Frequency));
                    newSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End, CultureInfo.InvariantCulture)), ocur.Frequency));
                    a.Series.Add(newSeries);
                    newSeries.TrackerKey = trackerKey.ToString();
                    ocur.trackerKey = trackerKey;
                    trackerKey++;
                    newSeries.MouseDown += foEvent_MouseDown;
                    if (ocur.Alarm == "YES")
                    {
                        var startPoint = new ScatterPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.Start, CultureInfo.InvariantCulture)), ocur.Frequency, 4, 0);
                        var endPoint = new ScatterPoint(DateTimeAxis.ToDouble(Convert.ToDateTime(ocur.End, CultureInfo.InvariantCulture)), ocur.Frequency, 4, 0);
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
                        fo.SelectedOccurrence = ocur;
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

        private SignalManager _signalMgr;
        public ResultMapViewModel ResultMapVM { get; set; }
        public List<EnergyFlowAreaCoordsMappingViewModel> Areas { get; set; }
    }
}
