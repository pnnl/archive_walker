using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Xml;
using System.Collections.ObjectModel;
using BAWGUI.Results.Models;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Core;
using BAWGUI.Utilities;
using BAWGUI.MATLABRunResults.Models;
using MapService.ViewModels;
using OxyPlot.Axes;
using OxyPlot.Series;
using DissipationEnergyFlowResults.ViewModels;

namespace BAWGUI.Results.ViewModels
{
    /// <summary>
    /// The ViewModel for a single Forced Oscillation event.
    /// TODO: Maybe change the name here. Too similar to ForcedOscillationResultsViewModel.
    /// </summary>
    public class ForcedOscillationResultViewModel : ViewModelBase
    {
        //public ForcedOscillationResultViewModel(ForcedOscillationType model, ForcedOscillationTypeOccurrence ocur)
        //{
        //    this._model = model;
        //    _occurrence = ocur;
        //}
        //public ForcedOscillationResultViewModel()
        //{
        //    _occurrences = new List<OccurrenceViewModel>();
        //    _filteredOccurrences = new ObservableCollection<OccurrenceViewModel>();
        //}
        public ForcedOscillationResultViewModel(DatedForcedOscillationEvent model)
        {
            this._model = model;
            _occurrences.Clear();
            _filteredOccurrences.Clear();
            foreach (var ocur in model.Occurrences)
            {
                var oc = new OccurrenceViewModel(ocur);
                _occurrences.Add(oc);
                _filteredOccurrences.Add(oc);
                oc.SelectedChannelChanged += _selectedChannelChanged;
                oc.SelectedPathChanged += _selectedPathChanged;
            }
            //_selectedOccurrence = new OccurrenceViewModel();
            _selectedOccurrence = FilteredOccurrences.FirstOrDefault();
        }
        public ForcedOscillationResultViewModel()
        {
            _model = new DatedForcedOscillationEvent();
        }
        private readonly DatedForcedOscillationEvent _model;
        public DatedForcedOscillationEvent Model
        {
            get { return this._model; }
        }
        private List<OccurrenceViewModel> _occurrences = new List<OccurrenceViewModel>();
        //private List<OccurrenceViewModel> _occurrences;
        public List<OccurrenceViewModel> Occurrences
        {
            get { return _occurrences; }
            set
            {
                //_model.Occurrences = value;
                _model.Occurrences.Clear();
                foreach (var ocr in value)
                {
                    _model.Occurrences.Add(ocr.Model);
                }
                _occurrences = value;
            }
        }
        private ObservableCollection<OccurrenceViewModel> _filteredOccurrences = new ObservableCollection<OccurrenceViewModel>();
        //private ObservableCollection<OccurrenceViewModel> _filteredOccurrences;
        public ObservableCollection<OccurrenceViewModel> FilteredOccurrences
        {
            get { return _filteredOccurrences; }
            set
            {
                _model.FilteredOccurrences.Clear();
                foreach (var ocr in value)
                {
                    _model.FilteredOccurrences.Add(ocr.Model);
                }
                _filteredOccurrences = value;
                //_model.FilteredOccurrences = new List<DatedOccurrence>();
                //foreach (var ocur in value)
                //{
                //    _model.FilteredOccurrences.Add(ocur.Model);
                //}
                //SelectedOccurrence = FilteredOccurrences.FirstOrDefault();
                OnPropertyChanged();
            }
        }
        public string Label
        {
            get { return "Event " + this._model.ID; }
        }
        public string OverallStartTime
        {
            get { return _model.OverallStartTime; }
        }
        public string OverallEndTime
        {
            get { return _model.OverallEndTime; }
        }
        public string ID
        {
            get { return _model.ID; }
        }
        public string Alarm
        {
            get { return _model.Alarm; }
        }
        //public string TypicalFrequency
        //{
        //    get { return _model.TypicalFrequency.ToString(); }
        //}
        //public string MaxPersistence
        //{
        //    get
        //    {
        //        if(float.IsNaN(_model.MaxPersistence))
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _model.MaxPersistence.ToString();
        //        }
        //    } 
        //}
        //public string MaxAmplitude
        //{
        //    get                 
        //    {
        //        if (float.IsNaN(_model.MaxAmplitude))
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _model.MaxAmplitude.ToString();
        //        }
        //    }
        //}
        //public string MaxSNR
        //{
        //    get
        //    {
        //        if (float.IsNaN(_model.MaxSNR))
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _model.MaxSNR.ToString();
        //        }
        //    }
        //}
        //public string MaxCoherence
        //{
        //    get
        //    {
        //        if (float.IsNaN(_model.MaxCoherence))
        //        {
        //            return "";
        //        }
        //        else
        //        {
        //            return _model.MaxCoherence.ToString();
        //        }
        //    }
        //}
        public float TypicalFrequency
        {
            get { return _model.TypicalFrequency; }
        }
        public float MaxPersistence
        {
            get
            {
                return _model.MaxPersistence;
            }
        }
        public float MaxAmplitude
        {
            get
            {
                return _model.MaxAmplitude;
            }
        }
        public float MaxSNR
        {
            get
            {
                return _model.MaxSNR;
            }
        }
        public float MaxCoherence
        {
            get
            {
                return _model.MaxCoherence;
            }
        }
        public int NumberOfOccurrences
        {
            get { return _filteredOccurrences.Count(); }
        }
        //private ForcedOscillationTypeOccurrence _occurrence = new ForcedOscillationTypeOccurrence();
        //public ForcedOscillationTypeOccurrence Occurrence
        //{
        //    get { return _occurrence; }
        //}

        //public string StartDate
        //{
        //    get { return _model.OverallStart.Split(null)[0]; }
        //}
        //public string StartTime
        //{
        //    get { return _model.OverallStart.Split(null)[1]; }
        //}
        //public string EndDate
        //{
        //    get { return _model.OverallEnd.Split(null)[0]; }
        //}
        //public string EndTime
        //{
        //    get { return _model.OverallEnd.Split(null)[1]; }
        //}
        //public string OccurrenceID
        //{
        //    get { return _occurrence.OccurrenceID.ToString(); }
        //}
        //public string Frequency
        //{
        //    get { return _occurrence.Frequency.ToString(); }
        //}
        //public string OcurStartDate
        //{
        //    get { return _occurrence.Start.Split(null)[0]; }
        //}
        //public string OcurStartTime
        //{
        //    get { return _occurrence.End.Split(null)[1]; }
        //}
        public string GetFirstStartOfFilteredOccurrences()
        {
            return _filteredOccurrences.OrderBy(x => x.Start).FirstOrDefault().Start;
        }
        public string GetLastEndOfFilteredOccurrences()
        {
            return _filteredOccurrences.OrderBy(x => x.End).LastOrDefault().End;
        }
        private OccurrenceViewModel _selectedOccurrence;
        public OccurrenceViewModel SelectedOccurrence
        {
            get { return _selectedOccurrence; }
            set
            {
                if (_selectedOccurrence != value)
                {
                    //save a reference to the previous selected occurrence and channel, 
                    //once the GUI update, the selected channel will be set to null due to change of itemssource of the datagrid
                    //this way, we can set it back to the value it had
                    OccurrenceViewModel oldSelectedOccurrence = null;
                    FOOccurrenceChannelViewModel oldSelectedChannel = null;
                    FOOccurrencePathViewModel oldSelectedPath = null;
                    if (_selectedOccurrence != null)
                    {
                        oldSelectedChannel = _selectedOccurrence.SelectedChannel;
                        oldSelectedPath = _selectedOccurrence.SelectedPath;
                        oldSelectedOccurrence = _selectedOccurrence;
                    }
                    _selectedOccurrence = value;
                    OnPropertyChanged();
                    OnSelectedOccurrenceChanged();
                    if (oldSelectedOccurrence != null)
                    {
                        oldSelectedOccurrence.SelectedChannel = oldSelectedChannel;
                        oldSelectedOccurrence.SelectedPath = oldSelectedPath;
                    }
                }
            }
        }
        public event EventHandler SelectedOccurrenceChanged;
        public virtual void OnSelectedOccurrenceChanged()
        {
            SelectedOccurrenceChanged?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler SelectedChannelChanged;
        public event EventHandler SelectedPathChanged;
        //public virtual void OnSelectedChannelChanged()
        //{
        //}
        private void _selectedChannelChanged(object sender, EventArgs e)
        {
            SelectedChannelChanged?.Invoke(this, EventArgs.Empty);
        }
        private void _selectedPathChanged(object sender, EventArgs e)
        {
            SelectedPathChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
