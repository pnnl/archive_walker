using BAWGUI.Core.Models;
using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.CoordinateMapping.Models
{
    public class EnergyFlowAreaCoordsMappingModel
    {
        public EnergyFlowAreaCoordsMappingModel()
        {
            Locations = new List<ConfigSite>();
        }
        private string _areaName;
        public string AreaName 
        {
            get { return _areaName; }
            set 
            {
                if (value.All(c => Char.IsLetterOrDigit(c) || c.Equals('_')))
                {
                    _areaName = value;
                }
                else
                {
                    throw new Exception("Area name can’t use spaces or special characters except underscore.");
                }
            } 
        }

        private bool _validateName(string value)
        {
            foreach (var c in value)
            {
                //sname.All(c => );
                if (Char.IsLetterOrDigit(c) || c.Equals('_'))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public SignalMapPlotType Type { get; set; }
        public List<ConfigSite> Locations { get; set; }
    }
}
