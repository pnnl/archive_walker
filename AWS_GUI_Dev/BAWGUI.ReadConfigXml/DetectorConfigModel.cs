using System.Collections.Generic;
using System.Xml.Linq;

namespace BAWGUI.ReadConfigXml
{
    public class DetectorConfigModel
    {
        private XElement _xElement;

        public DetectorConfigModel()
        {
        }

        public DetectorConfigModel(XElement xElement)
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
                DetectorList.Add(new PeriodogramDetectorModel(item));
            }
            item = _xElement.Element("Configuration").Element("SpectralCoherence");
            if (item != null)
            {
                DetectorList.Add(new SpectralCoherenceDetectorModel(item));
            }
            item = _xElement.Element("Configuration").Element("OutOfRangeGeneral");
            if (item != null)
            {
                DetectorList.Add(new OutOfRangeFrequencyDetectorModel(item));
            }
            item = _xElement.Element("Configuration").Element("WindRamp");
            if (item != null)
            {
                DetectorList.Add(new WindRampDetectorModel(item));
            }
            item = _xElement.Element("Configuration").Element("Ringdown");
            if (item != null)
            {
                DetectorList.Add(new RingdownDetectorModel(item));
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
                            AlarmingList.Add(new AlarmingPeriodogramModel(alarm));
                            break;
                        case "SpectralCoherence":
                            AlarmingList.Add(new AlarmingSpectralCoherenceModel(alarm));
                            break;
                        case "Ringdown":
                            AlarmingList.Add(new AlarmingRingdownModel(alarm));
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