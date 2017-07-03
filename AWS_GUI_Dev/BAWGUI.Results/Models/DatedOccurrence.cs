using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.Models
{
    public class DatedOccurrence
    {
        public DatedOccurrence(string dt, ForcedOscillationTypeOccurrence ocur)
        {
            _date = dt;
            _occurence = ocur;
        }
        private string _date;
        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }
        private ForcedOscillationTypeOccurrence _occurence;
        //public ForcedOscillationTypeOccurrence Occurence
        //{
        //    get { return _occurence; }
        //}
        public string Start
        {
            get { return _occurence.Start; }
        }
        public string End
        {
            get { return _occurence.End; }
        }
        public string ID
        {
            get { return _occurence.OccurrenceID.ToString(); }
        }
        public float Frequency
        {
            get { return (float)_occurence.Frequency; }
        }
        public float Persistence
        {
            get { return _occurence.Persistence; }
        }
        public bool Alarm
        {
            get
            {
                if (_occurence.AlarmFlag == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public List<ForcedOscillationTypeOccurrenceChannel> Channels
        {
            get { return _occurence.Channel.ToList(); }
        }
        public float MaxAmplitude
        {
            get { return _occurence.Channel.Select(x => x.Amplitude).Max(); }
        }
        public float MaxSNR
        {
            get { return _occurence.Channel.Select(x => x.SNR).Max(); }
        }
        public float MaxCoherence
        {
            get { return _occurence.Channel.Select(x => x.Coherence).Max(); }
        }
    }
}
