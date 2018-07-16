using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BAWGUI.MATLABRunResults.Models;
using BAWGUI.Xml;
using VoltageStability.Models;

namespace BAWGUI.Results.Models
{
    public class ResultsModel
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
            _outOfRangeEvents = new List<OutOfRangeEvent>();
            _windRampEvents = new List<WindRampEvent>();
            _voltageStabilityEvents = new List<VoltageStabilityEvent>();
        }

        internal void LoadResults(List<string> filenames, List<string> dates)
        {
            //_selectedStartTime = startDate;
            //_selectedEndTime = endDate;

            List<DatedForcedOscillationEvent> forcedOscillationCompleteList = new List<DatedForcedOscillationEvent>();
            var ringdownEvents = new List<RingDownEvent>();
            var outOfRangeEvents = new List<OutOfRangeEvent>();
            var windRampEvents = new List<WindRampEvent>();
            var voltageStabilityEvents = new List<VoltageStabilityEvent>();

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
                if (content.ForcedOscillation != null)
                {
                    foreach (var foe in content.ForcedOscillation)
                    {
                        var newfoe = new DatedForcedOscillationEvent(date, foe);
                        forcedOscillationCompleteList.Add(newfoe);
                    }
                }
                if (content.Ringdown != null)
                {
                    foreach (var rd in content.Ringdown)
                    {
                        var newrd = new RingDownEvent(rd, date);
                        ringdownEvents.Add(newrd);
                    }
                }
                if (content.OutOfRangeFrequency != null)
                {
                    foreach (var oor in content.OutOfRangeFrequency)
                    {
                        var newoor = new OutOfRangeEvent(oor, date);
                        outOfRangeEvents.Add(newoor);
                    }
                }
                if (content.WindRamp != null)
                {
                    foreach (var wr in content.WindRamp)
                    {
                        var newwr = new WindRampEvent(wr, date);
                        windRampEvents.Add(newwr);
                    }
                }
                if (content.Thevenin != null)
                {
                    foreach (var vs in content.Thevenin)
                    {
                        var newvs = new VoltageStabilityEvent(vs, date);
                        voltageStabilityEvents.Add(newvs);
                    }
                }
                stream.Close();
                //}
            }
            _combineForcedOscillationEvents(forcedOscillationCompleteList);
            _combineEventList(ringdownEvents, outOfRangeEvents, windRampEvents);
            if (voltageStabilityEvents.Count() > 0)
            {
                _combineVSEventList(voltageStabilityEvents);
            }
        }

        //private void _combineEventList(List<RingDownEvent> ringdownEvents, List<OutOfRangeEvent> outOfRangeEvents, List<WindRampEvent> windRampEvents)
        //{
        //    throw new NotImplementedException();
        //}

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
        private void _combineEventList(List<RingDownEvent> ringdownEvents, List<OutOfRangeEvent> outOfRangeEvents, List<WindRampEvent> windRampEvents)
        {
            var ringdownEventsCombined = new List<RingDownEvent>();
            var outOfRangeEventsCombined = new List<OutOfRangeEvent>();
            var windRampEventsCombined = new List<WindRampEvent>();

            var rdByID = ringdownEvents.GroupBy(x => x.ID).ToDictionary(p => p.Key, p => p.ToList());
            foreach (var rd in rdByID)
            {
                if (rd.Value.Count > 1)
                {
                    var keep = rd.Value.Aggregate((x1, x2) => DateTime.ParseExact(x1.Date, "yyMMdd", CultureInfo.InvariantCulture) > DateTime.ParseExact(x2.Date, "yyMMdd", CultureInfo.InvariantCulture) ? x1 : x2);
                    ringdownEventsCombined.Add(keep);
                }
                else
                {
                    ringdownEventsCombined.Add(rd.Value.FirstOrDefault());
                }
            }
            var oorByID = outOfRangeEvents.GroupBy(x => x.ID).ToDictionary(p => p.Key, p => p.ToList());
            foreach (var oor in oorByID)
            {
                if (oor.Value.Count > 1)
                {
                    var keep = oor.Value.Aggregate((x1, x2) => DateTime.ParseExact(x1.Date, "yyMMdd", CultureInfo.InvariantCulture) > DateTime.ParseExact(x2.Date, "yyMMdd", CultureInfo.InvariantCulture) ? x1 : x2);
                    outOfRangeEventsCombined.Add(keep);
                }
                else
                {
                    outOfRangeEventsCombined.Add(oor.Value.FirstOrDefault());
                }
            }
            var wrByID = windRampEvents.GroupBy(x => x.ID).ToDictionary(p => p.Key, p => p.ToList());
            foreach (var wr in wrByID)
            {
                if (wr.Value.Count > 1)
                {
                    var keep = wr.Value.Aggregate((x1, x2) => DateTime.ParseExact(x1.Date, "yyMMdd", CultureInfo.InvariantCulture) > DateTime.ParseExact(x2.Date, "yyMMdd", CultureInfo.InvariantCulture) ? x1 : x2);
                    windRampEventsCombined.Add(keep);
                }
                else
                {
                    windRampEventsCombined.Add(wr.Value.FirstOrDefault());
                }
            }

            _ringdownEvents = ringdownEventsCombined;
            _outOfRangeEvents = outOfRangeEventsCombined;
            _windRampEvents = windRampEventsCombined;
        }


        private void _combineVSEventList(List<VoltageStabilityEvent> voltageStabilityEvents)
        {
            var vsEventsCombined = new List<VoltageStabilityEvent>();
            var vsByID = voltageStabilityEvents.GroupBy(x => x.ID).ToDictionary(p => p.Key, p => p.ToList());
            foreach (var vs in vsByID)
            {
                if (vs.Value.Count > 1)
                {
                    var keep = vs.Value.Aggregate((x1, x2) => DateTime.ParseExact(x1.Date, "yyMMdd", CultureInfo.InvariantCulture) > DateTime.ParseExact(x2.Date, "yyMMdd", CultureInfo.InvariantCulture) ? x1 : x2);
                    vsEventsCombined.Add(keep);
                }
                else
                {
                    vsEventsCombined.Add(vs.Value.FirstOrDefault());
                }
            }
            _voltageStabilityEvents = vsEventsCombined;
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
        private List<OutOfRangeEvent> _outOfRangeEvents;
        public List<OutOfRangeEvent> OutOfRangeEvents
        {
            get { return _outOfRangeEvents; }
        }
        private List<WindRampEvent> _windRampEvents;
        public List<WindRampEvent> WindRampEvents
        {
            get { return _windRampEvents; }
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
        private List<VoltageStabilityEvent> _voltageStabilityEvents;
        public List<VoltageStabilityEvent> VoltageStabilityEvents
        {
            get { return _voltageStabilityEvents; }
        }

    }
}
