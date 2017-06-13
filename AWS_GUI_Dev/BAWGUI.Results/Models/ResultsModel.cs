using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BAWGUI.Xml;

namespace BAWGUI.Results.Models
{
    class ResultsModel
    {
        private EventSequenceType _events;

        public EventSequenceType Events
        {
            get { return this._events; }
            private set { this._events = value; }
        }

        internal void LoadResults(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EventSequenceType));
            FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            this.Events = serializer.Deserialize(stream) as EventSequenceType;
        }
    }
}
