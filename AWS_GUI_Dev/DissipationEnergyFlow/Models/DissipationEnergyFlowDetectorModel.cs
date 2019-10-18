using BAWGUI.CoordinateMapping.Models;
using BAWGUI.Core;
using System.Collections.Generic;

namespace DissipationEnergyFlow.Models
{
    public class DissipationEnergyFlowDetectorModel
    {
        public DissipationEnergyFlowDetectorModel()
        {
            Paths = new List<EnergyFlowPath>();
            Areas = new Dictionary<string, EnergyFlowAreaCoordsMappingModel>();
        }

        public string Name
        {
            get
            {
                return "Dissipation Energy Flow Detector";
            }
        }
        public int LocMinLength { get; set; }
        public int LocLengthStep { get; set; }
        public int LocRes { get; set; }
        public List<EnergyFlowPath> Paths { get; set; }
        public Dictionary<string, EnergyFlowAreaCoordsMappingModel> Areas { get; set; }
    }
    public class EnergyFlowPath
    {
        public EnergyFlowPath()
        {
            FromArea = new EnergyFlowAreaCoordsMappingModel();
            ToArea = new EnergyFlowAreaCoordsMappingModel();
            VoltageMag = new SignalSignatures();
            VoltageAng = new SignalSignatures();
            ActivePowerP = new SignalSignatures();
            ReactivePowerQ = new SignalSignatures();
        }
        public EnergyFlowAreaCoordsMappingModel FromArea { get; set; }
        public EnergyFlowAreaCoordsMappingModel ToArea { get; set; }
        public SignalSignatures VoltageMag { get; set; }
        public SignalSignatures VoltageAng { get; set; }
        public SignalSignatures ActivePowerP { get; set; }
        public SignalSignatures ReactivePowerQ { get; set; }
    }
}
