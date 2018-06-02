using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Core
{
    public class SignalSignatures
    {
        public SignalSignatures()
        {
            IsChecked = false;
            IsValid = true;
            IsEnabled = true;
            IsCustomSignal = false;
            IsNameTypeUnitChanged = false;
        }
        public string PMUName { get; set; }
        public string SignalName { get; set; }
        public string TypeAbbreviation { get; set; }
        public string Unit { get; set; }
        public string OldSignalName { get; set; }
        public string OldTypeAbbreviation { get; set; }
        public string OldUnit { get; set; }
        public bool? IsValid { get; set; }
        public bool? IsChecked { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsCustomSignal { get; set; }
        public bool? IsNameTypeUnitChanged { get; set; }
        public int SamplingRate { get; set; }
        public int PassedThroughDQFilter { get; set; }
        public int PassedThroughProcessor { get; set; }
    }
}
