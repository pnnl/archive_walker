using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoltageStability.Models
{
    public class VoltageStabilityEvent
    {
        public VoltageStabilityEvent(TheveninType vs, string date)
        {
            _thevenin = vs;
            Date = date;
        }
        public string Date { get; }
        private TheveninType _thevenin;
        public TheveninType OutOfRange
        {
            get { return _thevenin; }
        }
        public string ID
        {
            get { return _thevenin.ID.ToString(); }
        }
        public string Start
        {
            get { return _thevenin.Start; }
        }
        public string End
        {
            get { return _thevenin.End; }
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
        public List<TheveninTypeSub> Subs
        {
            get { return _thevenin.Sub.ToList(); }
        }

    }
}
