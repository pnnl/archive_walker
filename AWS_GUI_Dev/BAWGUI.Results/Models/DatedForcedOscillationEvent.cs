using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.Models
{
    public class DatedForcedOscillationEvent
    {
        public DatedForcedOscillationEvent(string d, ForcedOscillationType e)
        {
            _date = d;
            _forcedOscillationEvent = e;
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
            set { _forcedOscillationEvent = value; }
        }
    }
}
