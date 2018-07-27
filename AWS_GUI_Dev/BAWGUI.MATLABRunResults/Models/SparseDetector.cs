using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.MATLABRunResults.Models
{
    public class SparseDetector
    {
        public string Label;
        public SparseDetector()
        {
            _sparseSignals = new List<SparseSignal>();
        }
        private List<SparseSignal> _sparseSignals;
        public List<SparseSignal> SparseSignals
        {
            get { return _sparseSignals; }
            set { _sparseSignals = value; }
        }
        public string Type
        {
            get
            {
                if (_sparseSignals.Count()>0)
                {
                    var typ = _sparseSignals.FirstOrDefault().Type;
                    foreach (var item in _sparseSignals)
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
                if (_sparseSignals.Count() > 0)
                {
                    var unt = _sparseSignals.FirstOrDefault().Unit;
                    foreach (var item in _sparseSignals)
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
