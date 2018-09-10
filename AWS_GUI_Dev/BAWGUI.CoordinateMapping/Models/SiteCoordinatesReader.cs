using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BAWGUI.CoordinateMapping.ViewModels;
using BAWGUI.Core;
using BAWGUI.Xml;

namespace BAWGUI.CoordinateMapping.Models
{
    public class SiteCoordinatesReader
    {
        internal void ReadCoordsFile(string locationCoordinatesFilePath, ObservableCollection<SiteCoordinatesViewModel> coords)
        {
            coords.Clear();
            XmlSerializer serializer = new XmlSerializer(typeof(LocationCoordinatesConfig));
            FileStream stream = new FileStream(locationCoordinatesFilePath, FileMode.Open, FileAccess.Read);
            var content = serializer.Deserialize(stream) as LocationCoordinatesConfig;
            if (content.Coordinates != null)
            {
                foreach (var item in content.Coordinates)
                {
                    var lcl = new SiteCoordinatesModel(item);
                    coords.Add(new SiteCoordinatesViewModel(lcl));
                }
            }
        }
    }
}
