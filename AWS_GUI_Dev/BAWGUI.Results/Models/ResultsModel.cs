using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BAWGUI.Xml;

namespace BAWGUI.Results.Models
{
    class ResultsModel
    {
        //private List<EventSequenceType> _events = new List<EventSequenceType>();

        //public List<EventSequenceType> Events
        //{
        //    get { return this._events; }
        //    private set { this._events = value; }
        //}

        internal void LoadResults(List<string> filenames, string startDate, string endDate)
        {
            _selectedStartTime = startDate;
            _selectedEndTime = endDate;
            List<DatedForcedOscillationEvent> forcedOscillationCompleteList = new List<DatedForcedOscillationEvent>();
            foreach (var filename in filenames)
            {
                var date = Path.GetFileNameWithoutExtension(filename).Split('_').Last();
                if((string.Compare(date, startDate) >= 0 && string.Compare(date, endDate) <= 0) || date.ToLower() == "current")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(EventSequenceType));
                    FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    var content = serializer.Deserialize(stream) as EventSequenceType;
                    foreach (var foe in content.ForcedOscillation)
                    {
                        var newfoe = new DatedForcedOscillationEvent(date, foe);
                        forcedOscillationCompleteList.Add(newfoe);
                    }
                }
            }
            _combineForcedOscillationEvents(forcedOscillationCompleteList);
        }

        private void _combineForcedOscillationEvents(List<DatedForcedOscillationEvent> forcedOscillationCompleteList)
        {
            var foDictByID = forcedOscillationCompleteList.GroupBy(x => x.ForcedOscillationEvent.ID).ToDictionary(p=>p.Key, p=>p.ToList());
            foreach(var evnts in foDictByID)
            {
                var aEvent = evnts.Value.FirstOrDefault().ForcedOscillationEvent;
                var completeOccurencesList = new List<DatedOccurrence>();
                foreach (var evnt in evnts.Value)
                {
                    foreach(var ocur in evnt.ForcedOscillationEvent.Occurrence)
                    {
                        var newOcr = new DatedOccurrence(evnt.Date, ocur);
                        completeOccurencesList.Add(newOcr);
                    }
                }
                var occurByID = completeOccurencesList.GroupBy(x => x.Occurence.OccurrenceID).ToDictionary(p => p.Key, p => p.ToList());
                var singleOcur = (from ocur in occurByID where ocur.Value.Count == 1 select ocur.Value.FirstOrDefault()).ToList();
                var multiOcur = (from ocur in occurByID where ocur.Value.Count > 1 select ocur).ToList();
                var ocurList = (from a in singleOcur select a.Occurence).ToList();
                if (multiOcur.Count != 0)
                {
                    foreach (var ocur in multiOcur)
                    {
                        var latestDate = ocur.Value.Max(p => p.Date);
                        var item = ocur.Value.First(x => x.Date == latestDate);
                        ocurList.Add(item.Occurence);
                    }
                    ocurList = ocurList.OrderBy(x=>x.Start).ToList();
                }
                aEvent.Occurrence = ocurList.ToArray();
                _forcedOscillationCombinedList.Add(aEvent);
            }
        }
        
        private List<ForcedOscillationType> _forcedOscillationCombinedList = new List<ForcedOscillationType>();
        public List<ForcedOscillationType> ForcedOscillationCombinedList
        {
            get { return _forcedOscillationCombinedList; }
        }
        private string _selectedStartTime;
        public string SelectedStartTime
        {
            get { return _selectedStartTime; }
            set { _selectedStartTime = value; }
        }
        private string _selectedEndTime;
        public string SelectedEndTime
        {
            get { return _selectedEndTime; }
            set { _selectedEndTime = value; }
        }
    }
}
