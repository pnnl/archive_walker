using BAWGUI.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoltageStability.Models
{
    public class Site
    {
        public string Name { get; set; }
        public string StabilityThreshold { get; set; }
        public SignalSignatures Frequency { get; set; }
        public ObservableCollection<SignalSignatures> VoltageBuses { get; set; }
        public ObservableCollection<SignalSignatures> BranchesAndShunts { get; set; }
    }
    public class VoltageBus
    {
        public SignalSignatures Magnitude { get; set; }
        public SignalSignatures Angle { get; set; }
    }
    public class Branch
    {
        public SignalSignatures ActivePower { get; set; }
        public SignalSignatures ReactivePower { get; set; }
        public SignalSignatures CurrentMagnitude { get; set; }
        public SignalSignatures CurrentAngle { get; set; }
    }
    public class Shunt : Branch
    {

    }
}
