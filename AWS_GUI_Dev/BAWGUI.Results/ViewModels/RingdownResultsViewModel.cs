using BAWGUI.Results.Models;
using BAWGUI.RunMATLAB.ViewModels;
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
            _models = new List<RingDownEvent>();
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
                foreach (var model in value)
                {
                    _results.Add(new RingdownEventViewModel(model));
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
            //if (FilteredResults.Count() != 0)
            //{
            //    _drawFOPlot();
            //}
        }
    }
}
