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

        public ResultsModel()
        {
            _forcedOscillationCombinedList = new List<DatedForcedOscillationEvent>();
            _ringdownEvents = new List<RingDownEvent>();
        }

        internal void LoadResults(List<string> filenames, List<string> dates)
        {
            //_selectedStartTime = startDate;
            //_selectedEndTime = endDate;

            List<DatedForcedOscillationEvent> forcedOscillationCompleteList = new List<DatedForcedOscillationEvent>();

            foreach (var filename in filenames)
            {
                var date = Path.GetFileNameWithoutExtension(filename).Split('_').Last();
                if(date.ToLower() == "current")
                {
                    //dates.Sort();
                    //date = (Convert.ToInt32(Enumerable.LastOrDefault(dates)) + 1).ToString();
                    date = Enumerable.LastOrDefault(dates);
                }
                //if((string.Compare(date, startDate) >= 0 && string.Compare(date, endDate) <= 0) || date.ToLower() == "current")
                //{
                XmlSerializer serializer = new XmlSerializer(typeof(EventSequenceType));
                FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                var content = serializer.Deserialize(stream) as EventSequenceType;
                foreach (var foe in content.ForcedOscillation)
                {
                    var newfoe = new DatedForcedOscillationEvent(date, foe);
                    forcedOscillationCompleteList.Add(newfoe);
                }
                foreach (var rd in content.Ringdown)
                {
                    var newrd = new RingDownEvent(rd);
                    _ringdownEvents.Add(newrd);
                }
                //}
            }
            _combineForcedOscillationEvents(forcedOscillationCompleteList);
        }

        private void _combineForcedOscillationEvents(List<DatedForcedOscillationEvent> forcedOscillationCompleteList)
        {
            _forcedOscillationCombinedList.Clear();
            var foDictByID = forcedOscillationCompleteList.GroupBy(x => x.ID).ToDictionary(p=>p.Key, p=>p.ToList());
            foreach(var evnts in foDictByID)
            {
                var aEvent = evnts.Value.FirstOrDefault();
                var completeOccurencesList = new List<DatedOccurrence>();
                foreach (var evnt in evnts.Value)
                {
                    foreach(var ocur in evnt.Occurrences)
                    {
                        //var newOcr = new DatedOccurrence(evnt.Date, ocur);
                        completeOccurencesList.Add(ocur);
                    }
                }
                var occurByID = completeOccurencesList.GroupBy(x => x.ID).ToDictionary(p => p.Key, p => p.ToList());
                var singleOcur = (from ocur in occurByID where ocur.Value.Count == 1 select ocur.Value.FirstOrDefault()).ToList();
                var multiOcur = (from ocur in occurByID where ocur.Value.Count > 1 select ocur).ToList();
                //var ocurList = (from a in singleOcur select a.Occurence).ToList();
                if (multiOcur.Count != 0)
                {
                    foreach (var ocur in multiOcur)
                    {
                        var latestDate = ocur.Value.Max(p => p.Date);
                        var item = ocur.Value.First(x => x.Date == latestDate);
                        singleOcur.Add(item);
                    }
                    singleOcur = singleOcur.OrderBy(x=>x.Start).ToList();
                }
                aEvent.Occurrences = singleOcur;
                var ocurOrderedByEndTime = singleOcur.OrderBy(x => x.End).ToList();
                aEvent.OverallEndTime = ocurOrderedByEndTime.LastOrDefault().End;
                _forcedOscillationCombinedList.Add(aEvent);
            }
        }
        
        //private List<DatedForcedOscillationEvent> _forcedOscillationCombinedList = new List<DatedForcedOscillationEvent>();
        private List<DatedForcedOscillationEvent> _forcedOscillationCombinedList;
        public List<DatedForcedOscillationEvent> ForcedOscillationCombinedList
        {
            get { return _forcedOscillationCombinedList; }
        }
        private List<RingDownEvent> _ringdownEvents;
        public List<RingDownEvent> RingdownEvents
        {
            get { return _ringdownEvents; }
        }
        //private string _selectedStartTime;
        //public string SelectedStartTime
        //{
        //    get { return _selectedStartTime; }
        //    set { _selectedStartTime = value; }
        //}
        //private string _selectedEndTime;
        //public string SelectedEndTime
        //{
        //    get { return _selectedEndTime; }
        //    set { _selectedEndTime = value; }
        //}
    }
}
