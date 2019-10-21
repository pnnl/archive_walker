using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BAWGUI.CoordinateMapping.ViewModels;

namespace BAWGUI.CoordinateMapping.Models
{
    public class DEFAreaMappingConfigWriter
    {
        private Dictionary<string, EnergyFlowAreaCoordsMappingViewModel> _areas;

        public DEFAreaMappingConfigWriter(Dictionary<string, EnergyFlowAreaCoordsMappingViewModel> areas)
        {
            _areas = areas;
        }

        public XElement WriteConfigToXMLFormat()
        {
            var defAreaConfig = new XElement("DEFAreaMappingConfig", string.Empty);
            foreach (var pair in _areas)
            {
                var area = new XElement("Area", new XElement("AreaName", pair.Value.AreaName),
                                                new XElement("Type", pair.Value.Type.ToString()));
                var sites = new XElement("Sites");
                foreach (var sts in pair.Value.Locations)
                {
                    if (string.IsNullOrEmpty(sts.Name) || string.IsNullOrEmpty(sts.Latitude) || string.IsNullOrEmpty(sts.Longitude))
                    {
                        break;
                    }
                    else
                    {
                        var site = new XElement("Site", new XElement("Name", sts.Name),
                                                        new XElement("Latitude", sts.Latitude),
                                                        new XElement("Longitude", sts.Longitude));
                        sites.Add(site);
                    }
                }
                if (sites.HasElements)
                {
                    area.Add(sites);
                }
                else
                {
                    continue;
                }
                if (area.HasElements)
                {
                    defAreaConfig.Add(area);
                }
            }
            return defAreaConfig;
        }
    }
}
