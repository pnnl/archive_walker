using BAWGUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoltageStability.Models;

namespace VoltageStability.ViewModels
{
    public class VoltageStabilityDetectorViewModel:ViewModelBase
    {
        public VoltageStabilityDetectorViewModel()
        {
            _model = new VoltageStabilityDetector();
        }
        public VoltageStabilityDetectorViewModel(VoltageStabilityDetector model)
        {
            _model = model;
        }
        private VoltageStabilityDetector _model;
    }
}
