using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BAWGUI.CoordinateMapping.ViewModels;
using BAWGUI.Core.Models;
using BAWGUI.Core.Utilities;

namespace BAWGUI.CoordinateMapping.Models
{
    public class DEFAreaMappingConfigWriter
    {
        private List<EnergyFlowAreaCoordsMappingViewModel> _areas;

        public DEFAreaMappingConfigWriter(List<EnergyFlowAreaCoordsMappingViewModel> areas)
        {
            _areas = areas;
            _errorMessages = new List<string>();
        }

        public XElement WriteConfigToXMLFormat()
        {
            var defAreaConfig = new XElement("DEFAreaMappingConfig", string.Empty);
            foreach (var ar in _areas)
            {

                var type = ar.Type;
                var numberOfSites = ar.Locations.Count();
                bool dummySiteFound = false;
                var sites = new XElement("Sites");
                foreach (var sts in ar.Locations)
                {
                    if (sts == CoreUtilities.DummySiteCoordinatesModel)
                    {
                        dummySiteFound = true;
                        _errorMessages.Add(string.Format("Invalid site is setup with area {0}, of type {1}. This area {0} might be drawn incorrectly on the map", ar.AreaName, ar.Type.ToString()));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(sts.Name) || string.IsNullOrEmpty(sts.Latitude) || string.IsNullOrEmpty(sts.Longitude))
                        {
                            _errorMessages.Add(string.Format("Site {0} with latitude {1} and longitude {2} is set up with area {3}, which might be drawn incorrectly on the map", sts.Name, sts.Latitude, sts.Longitude, ar.AreaName));
                            continue;
                        }
                        else
                        {
                            var site = new XElement("Site", new XElement("Name", sts.Name),
                                                            new XElement("Latitude", sts.Latitude),
                                                            new XElement("Longitude", sts.Longitude));
                            sites.Add(site);
                        }
                    }
                }
                var siteCount = sites.Elements("Site").Count();
                if (dummySiteFound && siteCount == 1)
                {
                    _errorMessages.Add(string.Format("The mapping type of area {0} has been changed from {1} to Dot", ar.AreaName, type.ToString()));
                    type = SignalMapPlotType.Dot;
                    dummySiteFound = false;
                }
                if (dummySiteFound && siteCount == 2 && type != SignalMapPlotType.Line)
                {
                    _errorMessages.Add(string.Format("The mapping type of area {0} has been changed from {1} to Line", ar.AreaName, type.ToString()));
                    type = SignalMapPlotType.Line;
                    dummySiteFound = false;
                }
                var area = new XElement("Area", new XElement("AreaName", ar.AreaName),
                                                new XElement("Type",type));
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
        private List<string> _errorMessages;
        public List<string> GetErrorMessages()
        { 
            return _errorMessages;
        }
    }
}
