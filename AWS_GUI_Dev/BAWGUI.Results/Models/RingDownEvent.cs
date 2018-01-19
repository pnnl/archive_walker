using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.Models
{
    public class RingDownEvent
    {
        public RingDownEvent(RingdownType rd)
        {
            _ringDown = rd;
        }
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
        public List<RingdownTypeChannel> Channels
        {
            get { return _ringDown.Channel.ToList(); }
        }
    }
}
