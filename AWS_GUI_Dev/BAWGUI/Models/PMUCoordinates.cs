using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Xml;

namespace BAWGUI.Models
{
    public class PMUCoordinates
    {
        private ConfigSite item;

        public PMUCoordinates()
        {
            Name = "";
            Latitude = "0";
            Longitude = "0";
        }

        public PMUCoordinates(ConfigSite item)
        {
            Name = item.Name;
            Latitude = item.Latitude;
            Longitude = item.Longitude;
        }

        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        //public bool IsChecked { get; set; }
    }
}
