using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Xml;
namespace BAWGUI.Results.Models
{
    public class WindRampEvent
    {
        public WindRampEvent(WindRampType wr, string date)
        {
            _windRamp = wr;
            Date = date;
        }
        public string Date { get; }
        private WindRampType _windRamp;
        public WindRampType WindRamp
        {
            get { return _windRamp; }
        }
        public string ID
        {
            get { return _windRamp.ID.ToString(); }
        }
        public string TrendStart
        {
            get { return _windRamp.TrendStart; }
        }
        public string TrendEnd
        {
            get { return _windRamp.TrendEnd; }
        }
        public string TrendValue
        {
            get { return _windRamp.TrendValue.ToString(); }
        }
        public string PMU
        {
            get { return _windRamp.PMU; }
        }
        public string Channel
        {
            get { return _windRamp.Channel; }
        }
        public string Duration
        {
            get
            {
                var timeSpan = (DateTime.ParseExact(TrendEnd, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) - DateTime.ParseExact(TrendStart, "MM/dd/yy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture)).TotalMinutes;
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
        public float ValueStart
        {
            get { return (float)_windRamp.ValueStart; }
        }
        public float ValueEnd
        {
            get { return (float)_windRamp.ValueEnd; }
        }
    }
}
