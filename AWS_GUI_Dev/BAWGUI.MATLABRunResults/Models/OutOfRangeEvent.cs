using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.MATLABRunResults.Models
{
    public class OutOfRangeEvent
    {
        public OutOfRangeEvent(OutOfRangeFrequencyType oor, string date)
        {
            _outOfRange = oor;
            Date = date;
        }
        public string Date { get; }
        private OutOfRangeFrequencyType _outOfRange;
        public OutOfRangeFrequencyType OutOfRange
        {
            get { return _outOfRange; }
        }
        public string ID
        {
            get { return _outOfRange.ID.ToString(); }
        }
        public string Start
        {
            get { return _outOfRange.Start; }
        }
        public string End
        {
            get { return _outOfRange.End; }
        }
        public string Duration
        {
            get
            {
                var timeSpan = (DateTime.ParseExact(End, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) - DateTime.ParseExact(Start, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)).TotalMinutes;
                var rounded = 0d;
                var power = 0;
                while (rounded == 0)
                {
                    var scaled = timeSpan * Math.Pow(10, power);
                    rounded = Math.Round(scaled, 1) / Math.Pow(10, power);
                    power++;
                }
                return rounded.ToString();
                //return (DateTime.ParseExact(End, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) - DateTime.ParseExact(Start, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)).TotalMinutes.ToString();
            }
        }
        public string Extrema
        {
            get { return _outOfRange.Extrema; }
        }
        public string ExtremaFactor
        {
            get { return _outOfRange.ExtremaFactor; }
        }
        public List<OutOfRangeFrequencyTypeChannel> Channels
        {
            get { return _outOfRange.Channel.ToList(); }
        }
    }
}
