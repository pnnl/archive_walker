using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Xml;

namespace BAWGUI.Results.ViewModels
{
    /// <summary>
    /// The ViewModel for a single Forced Oscillation event.
    /// TODO: Maybe change the name here. Too similar to ForcedOscillationResultsViewModel.
    /// </summary>
    public class ForcedOscillationResultViewModel : ViewModelBase
    {
        public ForcedOscillationResultViewModel(ForcedOscillationType model, ForcedOscillationTypeOccurrence ocur)
        {
            this._model = model;
            _occurrence = ocur;
        }
        public ForcedOscillationResultViewModel(ForcedOscillationType model)
        {
            this._model = model;
            _occurrences.Clear();
            _filteredOccurrences.Clear();
            foreach (var ocur in model.Occurrence)
            {
                _occurrences.Add(new OccurrenceViewModel(ocur));
                _filteredOccurrences.Add(new OccurrenceViewModel(ocur));
            }
        }
        private readonly ForcedOscillationType _model;
        public ForcedOscillationType Model
        {
            get { return this._model; }
        }

        public string Label
        {
            get { return "Event " + this._model.ID; }
        }
        public string OverallStartTime
        {
            get { return _model.OverallStart; }
        }
        public string OverallEndTime
        {
            get { return _model.OverallEnd; }
        }
        public string ID
        {
            get { return _model.ID.ToString(); }
        }
        private ForcedOscillationTypeOccurrence _occurrence = new ForcedOscillationTypeOccurrence();
        public ForcedOscillationTypeOccurrence Occurrence
        {
            get { return _occurrence; }
        }
        private List<OccurrenceViewModel> _occurrences = new List<OccurrenceViewModel>();
        public List<OccurrenceViewModel> Occurrences
        {
            get { return _occurrences; }
            set
            {
                _occurrences = value;
            }
        }
        private List<OccurrenceViewModel> _filteredOccurrences = new List<OccurrenceViewModel>();
        public List<OccurrenceViewModel> FilteredOccurrences
        {
            get { return _filteredOccurrences; }
            set
            {
                _filteredOccurrences = value;
            }
        }
        public string StartDate
        {
            get { return _model.OverallStart.Split(null)[0]; }
        }
        public string StartTime
        {
            get { return _model.OverallStart.Split(null)[1]; }
        }
        public string EndDate
        {
            get { return _model.OverallEnd.Split(null)[0]; }
        }
        public string EndTime
        {
            get { return _model.OverallEnd.Split(null)[1]; }
        }
        public string OccurrenceID
        {
            get { return _occurrence.OccurrenceID.ToString(); }
        }
        public string Frequency
        {
            get { return _occurrence.Frequency.ToString(); }
        }
        public string OcurStartDate
        {
            get { return _occurrence.Start.Split(null)[0]; }
        }
        public string OcurStartTime
        {
            get { return _occurrence.End.Split(null)[1]; }
        }
    }
}
