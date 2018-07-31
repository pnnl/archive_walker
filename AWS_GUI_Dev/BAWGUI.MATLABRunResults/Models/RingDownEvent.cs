using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.MATLABRunResults.Models
{
    public class RingDownEvent
    {
        public RingDownEvent(RingdownType rd, string date)
        {
            _ringDown = rd;
            Date = date;
        }
        public string Date { get; }
        private RingdownType _ringDown;
        public RingdownType RingDown
        {
            get { return _ringDown; }
        }
        public string ID
        {
            get { return _ringDown.ID.ToString(); }
        }
        public string StartTime
        {
            get { return _ringDown.Start; }
        }
        public string EndTime
        {
            get { return _ringDown.End; }
        }
        public string Duration
        {
            get
            {
                var timeSpan = (DateTime.ParseExact(EndTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) - DateTime.ParseExact(StartTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
                var rounded = 0d;
                var power = 0;
                while (rounded == 0)
                {
                    var scaled = timeSpan * Math.Pow(10, power);
                    rounded = Math.Round(scaled, 1) / Math.Pow(10, power);
                    power++;
                }
                return rounded.ToString();
                //return Math.Round((DateTime.ParseExact(EndTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) - DateTime.ParseExact(StartTime, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds, 1).ToString();
            }
        }
        public List<RingdownTypeChannel> Channels
        {
            get { return _ringDown.Channel.ToList(); }
        }
    }
}
