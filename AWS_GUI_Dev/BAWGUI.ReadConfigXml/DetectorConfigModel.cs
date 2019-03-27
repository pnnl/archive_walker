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
            //var par = _xElement.Element("Configuration").Element("EventPath");
            //if (par != null)
            //{
            //    EventPath = par.Value;
            //}
            var par = _xElement.Element("Configuration").Element("ResultUpdateInterval");
            if (par != null)
            {
                ResultUpdateInterval = par.Value;
            }
            DetectorList = new List<object>();
            var items = _xElement.Element("Configuration").Elements("Periodogram");
            if (items != null)
            {
                foreach (var item in items)
                {
                    DetectorList.Add(new PeriodogramDetectorModel(item));
                }
            }
            items = _xElement.Element("Configuration").Elements("SpectralCoherence");
            if (items != null)
            {
                foreach (var item in items)
                {
                    DetectorList.Add(new SpectralCoherenceDetectorModel(item));
                }
            }
            items = _xElement.Element("Configuration").Elements("OutOfRangeGeneral");
            if (items != null)
            {
                foreach (var item in items)
                {
                    DetectorList.Add(new OutOfRangeFrequencyDetectorModel(item));
                }
            }
            items = _xElement.Element("Configuration").Elements("WindRamp");
            if (items != null)
            {
                foreach (var item in items)
                {
                    DetectorList.Add(new WindRampDetectorModel(item));
                }
            }
            items = _xElement.Element("Configuration").Elements("Ringdown");
            if (items != null)
            {
                foreach (var item in items)
                {
                    DetectorList.Add(new RingdownDetectorModel(item));
                }
            }
            items = _xElement.Element("Configuration").Elements("DataWriter");
            if (items != null)
            {
                foreach (var item in items)
                {
                    DetectorList.Add(new DataWriterDetectorModel(item));
                }
            }
            var alarms = _xElement.Element("Configuration").Element("Alarming");
            if (alarms != null)
            {
                AlarmingList = new List<object>();
                foreach (var alarm in alarms.Elements())
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