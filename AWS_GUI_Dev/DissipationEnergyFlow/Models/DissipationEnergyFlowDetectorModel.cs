using BAWGUI.CoordinateMapping.Models;
using BAWGUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DissipationEnergyFlow.Models
{
    public class DissipationEnergyFlowDetectorModel
    {
        public DissipationEnergyFlowDetectorModel()
        {
            Paths = new List<EnergyFlowPath>();
            //Areas = new Dictionary<string, EnergyFlowAreaCoordsMappingModel>();
            UniqueAreas = new List<string>();
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
        //public Dictionary<string, EnergyFlowAreaCoordsMappingModel> Areas { get; set; }
        public List<string> UniqueAreas { get; set; }
    }
    public class EnergyFlowPath
    {
        public EnergyFlowPath()
        {
            //FromArea = new EnergyFlowAreaCoordsMappingModel();
            //ToArea = new EnergyFlowAreaCoordsMappingModel();
            VoltageMag = new SignalSignatures();
            VoltageAng = new SignalSignatures();
            ActivePowerP = new SignalSignatures();
            ReactivePowerQ = new SignalSignatures();
        }
        //public EnergyFlowAreaCoordsMappingModel FromArea { get; set; }
        //public EnergyFlowAreaCoordsMappingModel ToArea { get; set; }
        private string _fromArea;
        public string FromArea
        {
            get { return _fromArea; }
            set
            {
                if (value.All(c => Char.IsLetterOrDigit(c) || c.Equals('_')))
                {
                    _fromArea = value;
                }
                else
                {
                    throw new Exception("Area name can’t use spaces or special characters except underscore.");
                }
            }
        }
        private string _toArea;
        public string ToArea
        {
            get { return _toArea; }
            set
            {
                if (value.All(c => Char.IsLetterOrDigit(c) || c.Equals('_')))
                {
                    _toArea = value;
                }
                else
                {
                    throw new Exception("Area name can’t use spaces or special characters except underscore.");
                }
            }
        }
        public SignalSignatures VoltageMag { get; set; }
        public SignalSignatures VoltageAng { get; set; }
        public SignalSignatures ActivePowerP { get; set; }
        public SignalSignatures ReactivePowerQ { get; set; }
    }
}
