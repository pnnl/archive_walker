using BAWGUI.CoordinateMapping.ViewModels;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace BAWGUI.CoordinateMapping.Models
{
    public class SiteCoordinatesWriter
    {
        internal void WriteCoordsFile(string filePath, ObservableCollection<SiteCoordinatesViewModel> siteCoords)
        {
            var config = new XElement("Config");
            var coords = new XElement("Coordinates");
            foreach (var item in siteCoords)
            {
                var newLocation = new XElement("Site", new XElement("Name", item.SiteName),
                                                        new XElement("Latitude", item.Latitude),
                                                        new XElement("Longitude", item.Longitude));
                coords.Add(newLocation);
            }
            config.Add(coords);
            config.Save(filePath);
        }
    }
}
