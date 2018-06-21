using System.Collections.Generic;
using System.Xml.Linq;

namespace BAWGUI.ReadConfigXml
{
    public class DetectorConfig
    {
        private XElement _xElement;

        public DetectorConfig(XElement xElement)
        {
            this._xElement = xElement;
            var par = _xElement.Element("Configuration").Element("EventPath");
            if (par != null)
            {
                EventPath = par.Value;
            }
            par = _xElement.Element("Configuration").Element("ResultUpdateInterval");
            if (par != null)
            {
                ResultUpdateInterval = par.Value;
            }
            DetectorList = new List<object>();
            var item = _xElement.Element("Configuration").Element("Periodogram");
            if (item != null)
            {
                DetectorList.Add(new PeriodogramDetector(item));
            }
            item = _xElement.Element("Configuration").Element("SpectralCoherence");
            if (item != null)
            {
                DetectorList.Add(new SpectralCoherenceDetector(item));
            }
            item = _xElement.Element("Configuration").Element("OutOfRangeGeneral");
            if (item != null)
            {
                DetectorList.Add(new OutOfRangeFrequencyDetector(item));
            }
            item = _xElement.Element("Configuration").Element("WindRamp");
            if (item != null)
            {
                DetectorList.Add(new WindRampDetector(item));
            }
            item = _xElement.Element("Configuration").Element("Ringdown");
            if (item != null)
            {
                DetectorList.Add(new RingdownDetector(item));
            }
            item = _xElement.Element("Configuration").Element("Alarming");
            if (item != null)
            {
                AlarmingList = new List<object>();
                foreach (var alarm in item.Elements())
                {
                    switch (alarm.Name.LocalName)
                    {
                        case "Periodogram":
                            AlarmingList.Add(new AlarmingPeriodogram(alarm));
                            break;
                        case "SpectralCoherence":
                            AlarmingList.Add(new AlarmingSpectralCoherence(alarm));
                            break;
                        case "Ringdown":
                            AlarmingList.Add(new AlarmingRingdown(alarm));
                            break;
                        default:
                            throw new System.Exception("Unknown alarming detector.");
                    }
                }
            }
        }
        public string EventPath { get; set; }
        public string ResultUpdateInterval { get; set; }
        public List<object> DetectorList { get; set; }
        public List<object> AlarmingList { get; set; }
    }
}