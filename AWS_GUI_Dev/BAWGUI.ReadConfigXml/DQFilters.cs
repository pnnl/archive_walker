using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BAWGUI.ReadConfigXml
{
    public class DQFilter
    {
        private XElement item;

        public DQFilter(XElement item)
        {
            this.item = item;
            PMUElementList = PMUElementReader.ReadPMUElements(item);
        }
        public string SetToNaN { get; set; } = "True";
        public string Name { get; set; }
        public List<PMUElement> PMUElementList { get; set; }
    }

    public class StatusFlagsDQFilter : DQFilter
    {
        public StatusFlagsDQFilter(XElement item) : base(item)
        {
            Name = "Status Flags";
        }
    }
    public class ZerosDQFilter : DQFilter
    {
        public ZerosDQFilter(XElement item) : base(item)
        {
            Name = "Zeros";
        }
    }
    public class MissingDQFilter : DQFilter
    {
        public MissingDQFilter(XElement item) : base(item)
        {
            Name = "Missing";
        }
    }
    public class WrappingFailureDQFilter : DQFilter
    {
        public WrappingFailureDQFilter(XElement item) : base(item)
        {
            Name = "Angle Wrapping";
            var par = item.Element("Parameters").Element("AngleThresh").Value;
            if (par != null)
            {
                AngleThresh = par;
            }
        }
        public string AngleThresh { get; set; }
    }

    public class DataFrameDQFilter : DQFilter
    {
        public DataFrameDQFilter(XElement item) : base(item)
        {
            Name = "Data Frame";
            var par = item.Element("Parameters").Element("PercentBadThresh").Value;
            if (par != null)
            {
                PercentBadThresh = par;
            }
        }
        public string PercentBadThresh { get; set; }
    }
    public class PMUchanDQFilter : DataFrameDQFilter
    {
        public PMUchanDQFilter(XElement item) : base(item)
        {
            Name = "Channel";
        }
    }
    public class PMUallDQFilter : DataFrameDQFilter
    {
        public PMUallDQFilter(XElement item) : base(item)
        {
            Name = "Entire PMU";
        }
    }

    public class StaleDQFilter : DQFilter
    {
        public StaleDQFilter(XElement item) : base(item)
        {
            Name = "Stale Data";
            var par = item.Element("Parameters").Element("StaleThresh").Value;
            if (par != null)
            {
                StaleThresh = par;
            }
            par = item.Element("Parameters").Element("FlagAllByFreq").Value;
            if (par != null)
            {
                if (par.ToLower()=="true")
                {
                    FlagAllByFreq = true;
                }
                else
                {
                    FlagAllByFreq = false;
                }
            }
        }
        public string StaleThresh { get; set; }
        public bool FlagAllByFreq { get; set; }
    }

    public class OutlierDQFilter : DQFilter
    {
        public OutlierDQFilter(XElement item) : base(item)
        {
            Name = "Outliers";
            var par = item.Element("Parameters").Element("StdDevMult").Value;
            if (par != null)
            {
                StdDevMult = par;
            }
        }
        public string StdDevMult { get; set; }
    }

    public class FreqDQFilter : DQFilter
    {
        public FreqDQFilter(XElement item) : base(item)
        {
            Name = "Nominal Frequency";
            var par = item.Element("Parameters").Element("FreqMaxChan").Value;
            if (par != null)
            {
                FreqMaxChan = par;
            }
            par = item.Element("Parameters").Element("FreqMinChan").Value;
            if (par != null)
            {
                FreqMinChan = par;
            }
            par = item.Element("Parameters").Element("FreqPctChan").Value;
            if (par != null)
            {
                FreqPctChan = par;
            }
            par = item.Element("Parameters").Element("FreqMinSamp").Value;
            if (par != null)
            {
                FreqMinSamp = par;
            }
            par = item.Element("Parameters").Element("FreqMaxSamp").Value;
            if (par != null)
            {
                FreqMaxSamp = par;
            }
        }
        public string FreqMaxChan { get; set; }
        public string FreqMinChan { get; set; }
        public string FreqPctChan { get; set; }
        public string FreqMinSamp { get; set; }
        public string FreqMaxSamp { get; set; }
    }

    public class VoltPhasorDQFilter : DQFilter
    {
        public VoltPhasorDQFilter(XElement item) : base(item)
        {
            Name = "Nominal Voltage";
            var par = item.Element("Parameters").Element("NomVoltage").Value;
            if (par != null)
            {
                NomVoltage = par;
            }
            par = item.Element("Parameters").Element("VoltMin").Value;
            if (par != null)
            {
                VoltMin = par;
            }
            par = item.Element("Parameters").Element("VoltMax").Value;
            if (par != null)
            {
                VoltMax = par;
            }
        }
        public string VoltMin { get; set; }
        public string VoltMax { get; set; }
        public string NomVoltage { get; set; }
    }
}
