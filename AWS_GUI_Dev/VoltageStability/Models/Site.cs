using BAWGUI.Core;
using BAWGUI.SignalManagement.Models;
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
        public Site()
        {
            Frequency = new Signal();
            VoltageBuses = new List<VoltageBus>();
            BranchesAndShunts = new List<Branch>();
        }
        public string Name { get; set; }
        public string StabilityThreshold { get; set; }
        public Signal Frequency { get; set; }
        public List<VoltageBus> VoltageBuses { get; set; }
        public List<Branch> BranchesAndShunts { get; set; }
    }
    public class VoltageBus
    {
        public VoltageBus(Signal s1, Signal s2)
        {
            Magnitude = s1;
            Angle = s2;
        }
        public Signal Magnitude { get; set; }
        public Signal Angle { get; set; }
    }
    public class Branch
    {
        public Branch() { }
        public Signal ActivePower { get; set; }
        public Signal ReactivePower { get; set; }
        public Signal CurrentMagnitude { get; set; }
        public Signal CurrentAngle { get; set; }
    }
    public class Shunt : Branch
    {

    }
}
