using System.Collections.Generic;
using System.Linq;

namespace BAWGUI.MATLABRunResults.Models
{
    public class OutOfRangeDetector
    {
        public string Label;
        public OutOfRangeDetector()
        {
            _oorSignals = new List<OutOfRangeSignal>();

        }
        private List<OutOfRangeSignal> _oorSignals;
        public List<OutOfRangeSignal> OORSignals
        {
            get { return _oorSignals; }
            set { _oorSignals = value; }
        }
        public string Type
        {
            get
            {
                if (_oorSignals.Count() > 0)
                {
                    var typ = _oorSignals.FirstOrDefault().Type;
                    foreach (var item in _oorSignals)
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
                if (_oorSignals.Count() > 0)
                {
                    var unt = _oorSignals.FirstOrDefault().Unit;
                    foreach (var item in _oorSignals)
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