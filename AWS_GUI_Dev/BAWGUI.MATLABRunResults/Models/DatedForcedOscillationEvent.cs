﻿using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.MATLABRunResults.Models
{
    public class DatedForcedOscillationEvent
    {
        //public DatedForcedOscillationEvent()
        //{
        //    _occurrences = new List<DatedOccurrence>();
        //    _filteredOccurrences = new List<DatedOccurrence>();
        //    _forcedOscillationEvent = new ForcedOscillationType();
        //}
        public DatedForcedOscillationEvent(string dt, ForcedOscillationType foevent)
        {
            _date = dt;
            _forcedOscillationEvent = foevent;
            _occurrences.Clear();
            _filteredOccurrences.Clear();
            foreach (var ocur in foevent.Occurrence)
            {
                var doc = new DatedOccurrence(_date, ocur);
                _occurrences.Add(doc);
                _filteredOccurrences.Add(doc);
            }
        }

        public DatedForcedOscillationEvent()
        {
        }

        private string _date;
        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }
        private ForcedOscillationType _forcedOscillationEvent;
        public ForcedOscillationType ForcedOscillationEvent
        {
            get { return _forcedOscillationEvent; }
        }
        private List<DatedOccurrence> _occurrences = new List<DatedOccurrence>();
        public List<DatedOccurrence> Occurrences
        {
            get { return _occurrences; }
            set
            {
                _occurrences = value;
            }
        }
        private List<DatedOccurrence> _filteredOccurrences = new List<DatedOccurrence>();
        public List<DatedOccurrence> FilteredOccurrences
        {
            get { return _filteredOccurrences; }
            set
            {
                _filteredOccurrences = value;
            }
        }
        public string OverallStartTime
        {
            get { return _forcedOscillationEvent.OverallStart; }
        }
        public string OverallEndTime
        {
            get { return _forcedOscillationEvent.OverallEnd; }
            set { _forcedOscillationEvent.OverallEnd = value; }
        }
        public string ID
        {
            get { return _forcedOscillationEvent.ID.ToString(); }
        }
        public string Alarm
        {
            get
            {
                if (_filteredOccurrences.Select(x => x.Alarm).Contains(true))
                {
                   return "YES";
                }
                else
                {
                    return "NO";
                }
            }
        }
        public float TypicalFrequency
        {
            get { return _findMedian(_filteredOccurrences.Select(x => x.Frequency).ToList()); }
        }
        public float MaxPersistence
        {
            get
            {
                if (_filteredOccurrences != null && _filteredOccurrences.Count() > 0)
                {
                    return _filteredOccurrences.Select(x => x.Persistence).Max();
                }
                else {
                    return float.NaN;
                }
            }
        }
        public float MaxAmplitude
        {
            get
            {
                if (_filteredOccurrences != null && _filteredOccurrences.Count() > 0)
                {
                    return _filteredOccurrences.Select(x => x.MaxAmplitude).Max();
                }
                else
                {
                    return float.NaN;
                }
            }
        }
        public float MaxSNR
        {
            get
            {
                if (_filteredOccurrences != null && _filteredOccurrences.Count() > 0)
                {
                    return _filteredOccurrences.Select(x => x.MaxSNR).Max();
                }
                else
                {
                    return float.NaN;
                }
            }
        }
        public float MaxCoherence
        {
            get
            {
                if (_filteredOccurrences != null && _filteredOccurrences.Count() > 0)
                {
                    return _filteredOccurrences.Select(x => x.MaxCoherence).Max();
                }
                else
                {
                    return float.NaN;
                }
            }
        }
        //public int NumberOfCoherences
        //{
        //    get { return _occurrences.Count(); }
        //}
        private float _findMedian(List<float> list)
        {
            float medianFrequency = 0;
            list.Sort();
            int count = list.Count();
            if(count ==0)
            {
                throw new Exception("Frequency list is empty!");
            }
            else if(count%2 == 0)
            {
                medianFrequency = (list[count/2] + list[(count/2 -1)]) / 2;
            }
            else
            {
                medianFrequency = list[count / 2];
            }
            return medianFrequency;
        }
    }
}
