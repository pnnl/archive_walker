using System;
using System.IO;
using System.Xml.Serialization;
using BAWGUI.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BAWGUI.Test
{
    [TestClass]
    public class XmlTest
    {
        [TestMethod]
        public void AllTypes()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EventSequenceType));
            FileStream stream = new FileStream(@"Data\EventList_AllTypes.xml", FileMode.Open, FileAccess.Read);
            var events = serializer.Deserialize(stream) as EventSequenceType;
            Assert.AreEqual(2, events.ForcedOscillation.Length, "Incorrect number of forced oscillations.");
            Assert.AreEqual(2, events.Ringdown.Length, "Incorrect number of ringdowns.");
            Assert.AreEqual(1, events.OutOfRangeFrequency.Length, "Incorrect number of out of range frequencies");
            Assert.AreEqual(1, events.WindRamp.Length, "Incorrect number of wind ramps.");
        }

        [TestMethod]
        public void MissingWindRamp()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EventSequenceType));
            FileStream stream = new FileStream(@"Data\EventList_MissingWindRamp.xml", FileMode.Open, FileAccess.Read);
            var events = serializer.Deserialize(stream) as EventSequenceType;
            Assert.AreEqual(2, events.ForcedOscillation.Length, "Incorrect number of forced oscillations.");
            Assert.AreEqual(2, events.Ringdown.Length, "Incorrect number of ringdowns.");
            Assert.AreEqual(1, events.OutOfRangeFrequency.Length, "Incorrect number of out of range frequencies.");
            Assert.IsNull(events.WindRamp, "Wind ramp should be null");
        }
    }
}
