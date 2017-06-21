using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Xml;
using System.Windows;

namespace BAWGUI.Results.ViewModels
{
    /// <summary>
    /// The ViewModel for the full Forced Oscillation results view.
    /// </summary>
    public class ForcedOscillationResultsViewModel : ViewModelBase
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //private ForcedOscillationType[] _models;
        private List<ForcedOscillationType> _models = new List<ForcedOscillationType>();
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
        public List<ForcedOscillationType> Models
        {
            get { return this._models; }
            set
            {
                //_models = value;
                _results.Clear();
                _filteredResults.Clear();
                foreach (var model in value)
                {
                    //_results.Add(new ForcedOscillationResultViewModel(model));
                    //_filteredResults.Add(new ForcedOscillationResultViewModel(model));


                    //flattened occurrence with events
                    foreach (var ocur in model.Occurrence)
                    {
                        _results.Add(new ForcedOscillationResultViewModel(model, ocur));
                        _filteredResults.Add(new ForcedOscillationResultViewModel(model, ocur));
                    }
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
                if(!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(_selectedEndTime))
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
            foreach ( var evnt in _results)
            {
                DateTime st = DateTime.Parse(evnt.OverallStartTime);
                DateTime ed = DateTime.Parse(evnt.OverallEndTime);
                if (DateTime.Compare(st, endT) <= 0 && DateTime.Compare(ed, startT) >= 0 )
                {
                    //List<OccurrenceViewModel> newOcurs = new List<OccurrenceViewModel>();
                    //foreach (var ocur in evnt.Occurrences)
                    //{
                    //    DateTime ocurst = DateTime.Parse(ocur.Start);
                    //    DateTime ocured = DateTime.Parse(ocur.End);
                    //    if (DateTime.Compare(ocurst, endT) <= 0 && DateTime.Compare(ocured, startT) >= 0)
                    //    {
                    //        newOcurs.Add(ocur);
                    //    }
                    //}
                    //if (newOcurs.Count != 0)
                    //{
                    //    evnt.FilteredOccurrences = newOcurs;
                    //    newResults.Add(evnt);
                    //}

                    //flattened occurence with events
                    DateTime ocurst = DateTime.Parse(evnt.Occurrence.Start);
                    DateTime ocured = DateTime.Parse(evnt.Occurrence.End);
                    if (DateTime.Compare(ocurst, endT) <= 0 && DateTime.Compare(ocured, startT) >= 0)
                    {
                        newResults.Add(evnt);
                    }
                }
            }
            FilteredResults = newResults;
        }
    }
}
