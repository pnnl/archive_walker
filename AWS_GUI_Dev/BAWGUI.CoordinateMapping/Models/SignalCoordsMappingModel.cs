using BAWGUI.Core.Models;
using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.CoordinateMapping.Models
{
    public class SignalCoordsMappingModel
    {
        public string PMUName { get; set; }
        public string SignalName { get; set; }
        public SignalMapPlotType Type { get; set; }
        public List<ConfigSite> Locations { get; set; }
    }
}
