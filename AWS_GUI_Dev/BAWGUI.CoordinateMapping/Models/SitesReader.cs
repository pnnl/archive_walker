using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BAWGUI.CoordinateMapping.Models
{
    public class SitesReader
    {
        public static List<ConfigSite> ReadSites(XElement signal)
        {
            var sites = new List<ConfigSite>();
            XmlSerializer serializer = null;
            try
            {
                serializer = new XmlSerializer(typeof(ConfigSite), new XmlRootAttribute("Site"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (serializer != null)
            {
                foreach (var item in signal.Element("Sites").Elements("Site"))
                {
                    var b = item.CreateReader();
                    ConfigSite a = null;
                    try
                    {
                        a = (ConfigSite)serializer.Deserialize(b);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    if (a != null)
                    {
                        sites.Add(a);
                    }
                }
            }

            return sites;
        }
    }
}
