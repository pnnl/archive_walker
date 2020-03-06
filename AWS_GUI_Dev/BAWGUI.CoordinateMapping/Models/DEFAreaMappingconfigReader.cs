using BAWGUI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BAWGUI.CoordinateMapping.Models
{
    public class DEFAreaMappingconfigReader
    {
        private XElement _config;
        public DEFAreaMappingconfigReader()
        {
            DEFAreaMappingList = new List<EnergyFlowAreaCoordsMappingModel>();
        }
        public DEFAreaMappingconfigReader(XElement xElement) : this()
        {
            _config = xElement;
            _readDEFAreaMappingConfig();
        }
        private void _readDEFAreaMappingConfig()
        {
            if (_config != null)
            {
                var areas = _config.Elements("Area");
                foreach (var area in areas)
                {
                    var newArea = _readArea(area);
                    DEFAreaMappingList.Add(newArea);
                }
            }
        }

        private EnergyFlowAreaCoordsMappingModel _readArea(XElement area)
        {
            var result = new EnergyFlowAreaCoordsMappingModel();
            var an = area.Element("AreaName");
            if (an != null)
            {
                result.AreaName = an.Value;
            }
            var tp = area.Element("Type");
            if (tp != null)
            {
                result.Type = (SignalMapPlotType)Enum.Parse(typeof(SignalMapPlotType), tp.Value);
            }
            result.Locations = SitesReader.ReadSites(area);
            return result;
        }

        public List<EnergyFlowAreaCoordsMappingModel> DEFAreaMappingList { get; set; }
        public List<EnergyFlowAreaCoordsMappingModel> GetDEFAreaMappoingModel()
        {
            return DEFAreaMappingList;
        }
    }
}
