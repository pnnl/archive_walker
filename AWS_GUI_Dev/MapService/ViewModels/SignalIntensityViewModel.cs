using BAWGUI.Core;
using BAWGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapService.ViewModels
{
    public class SignalIntensityViewModel : ViewModelBase
    {
        public SignalIntensityViewModel(SignalSignatureViewModel signal, double rule)
        {
            this.Signal = signal;
            this.Intensity = rule;
        }

        public SignalSignatureViewModel Signal { get; set; }
        public double Intensity { get; set; }
    }
}
