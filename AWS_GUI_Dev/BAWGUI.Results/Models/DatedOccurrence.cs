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
        public DatedOccurrence(string d, ForcedOscillationTypeOccurrence e)
        {
            _date = d;
            _occurence = e;
        }
        private string _date;
        private ForcedOscillationTypeOccurrence _occurence;
        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public ForcedOscillationTypeOccurrence Occurence
        {
            get { return _occurence; }
            set { _occurence = value; }
        }
    }
}
