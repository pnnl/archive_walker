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
        }

        private ObservableCollection<ForcedOscillationResultViewModel> _results = new ObservableCollection<ForcedOscillationResultViewModel>();
        private ObservableCollection<ForcedOscillationResultViewModel> _filteredResults = new ObservableCollection<ForcedOscillationResultViewModel>();
        public ObservableCollection<ForcedOscillationResultViewModel> FilteredResults
        {
            get { return _filteredResults; }
            set
            {
                _filteredResults = value;
                OnPropertyChanged();
            }
        }
        private List<DatedForcedOscillationEvent> _models = new List<DatedForcedOscillationEvent>();
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
                _channelTableWin.Show();
            }
        }
    }
}
