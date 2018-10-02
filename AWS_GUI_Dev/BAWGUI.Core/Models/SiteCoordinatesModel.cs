using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Xml;

namespace BAWGUI.Core
{
    public class SiteCoordinatesModel
    {
        //private ConfigSite item;

        public SiteCoordinatesModel()
        {
            Name = "";
            Latitude = "";
            Longitude = "";
            _internalCounter += 1;
            _internalID = _internalCounter;
        }

        public SiteCoordinatesModel(ConfigSite item) : this(item.Name)
        {
            Latitude = item.Latitude;
            Longitude = item.Longitude;
        }

        public SiteCoordinatesModel(string name) : this()
        {
            Name = name;
        }

        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        private static int _internalCounter  = 0;
        private int _internalID;

        public int GetInternalID()
        {
            return _internalID;
        }
        //public bool IsChecked { get; set; }
    }
}
