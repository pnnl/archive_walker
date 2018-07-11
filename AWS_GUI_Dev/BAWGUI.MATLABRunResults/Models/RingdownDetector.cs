using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.MATLABRunResults.Models
{
    public class RingdownDetector
    {
        public string Label;
        public RingdownDetector()
        {
            _ringdownSignals = new List<RingdownSignal>();

        }
        private List<RingdownSignal> _ringdownSignals;
        public List<RingdownSignal> RingdownSignals
        {
            get { return _ringdownSignals; }
            set { _ringdownSignals = value; }
        }
        public string Type
        {
            get
            {
                if (_ringdownSignals.Count() > 0)
                {
                    var typ = _ringdownSignals.FirstOrDefault().Type;
                    foreach (var item in _ringdownSignals)
                    {
                        if (item.Type != typ)
                        {
                            return "Mixed";
                        }
                    }
                    return typ;
                }
                return "";
            }
        }
        public string Unit
        {
            get
            {
                if (_ringdownSignals.Count() > 0)
                {
                    var unt = _ringdownSignals.FirstOrDefault().Unit;
                    foreach (var item in _ringdownSignals)
                    {
                        if (item.Unit != unt)
                        {
                            return "Mixed";
                        }
                    }
                    return unt;
                }
                return "";
            }
        }
    }
}
