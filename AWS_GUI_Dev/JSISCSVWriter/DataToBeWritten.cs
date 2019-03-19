using BAWGUI.Core;
using BAWGUI.Utilities;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSISCSVWriter
{
    public class DataToBeWritten
    {
        public DataToBeWritten()
        {
            NameRowList = new List<string>() { "Time" };
            TypeRowList = new List<string>() { "Type" };
            UnitRowList = new List<string>() { "Second" };
            PMUList = new List<string>() { "Time" };
        }
        public DataToBeWritten(List<Signal> signals) :  this()
        {
            InputSignals = signals;
            ReArrangeSignal();
        }
        public Matrix<double> Data;
        /// <summary>
        ///     ''' first row of the csv file, all signal names
        ///     ''' </summary>
        public List<string> NameRowList;
        /// <summary>
        ///     ''' second row of the csv file, all signal types
        ///     ''' </summary>
        public List<string> TypeRowList;
        /// <summary>
        ///     ''' third row of the csv file, all signal units
        ///     ''' </summary>
        public List<string> UnitRowList;
        /// <summary>
        ///     ''' fourth row of the csv file, the full standard name of the signal:
        ///     ''' 16-character PMU name . 16-character signal name . suffix
        ///     ''' </summary>
        public List<string> PMUList;
        public List<Signal> InputSignals { get; set; }
        public void ReArrangeSignal()
        {
            if (InputSignals.Count != 0)
            {
                var firstTimeStamp = DateTime.FromOADate(InputSignals.FirstOrDefault().TimeStampNumber.FirstOrDefault()).ToString(@"yyyyMMdd_HHmmss");
                var firstDateTime = DateTime.ParseExact(firstTimeStamp, "yyyyMMdd_HHmmss", null, System.Globalization.DateTimeStyles.None).ToOADate();// - new DateTime(0001, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                var timeArr = InputSignals.FirstOrDefault().TimeStampNumber.ToArray();
                //var firstTimeStamp2 = DateTime.FromOADate(InputSignals.FirstOrDefault().TimeStampNumber.FirstOrDefault()).ToString(@"yyyyMMdd_HHmmss.ffffff");
                //Data = Matrix<double>.Build.Dense(timeArr.Count(), 1, (i, j) => (double)i / (double)InputSignals.FirstOrDefault().SamplingRate);
                Data = Matrix<double>.Build.Dense(timeArr.Count(), 1, (i, j) => (timeArr[i] - firstDateTime)*86400);
                var orderedSignal = InputSignals.OrderBy(x => x.PMUname);
                foreach (var signal in orderedSignal)
                {
                    NameRowList.Add(signal.SignalName);
                    TypeRowList.Add(_typeConverter(signal.Type));
                    UnitRowList.Add(_unitConverter(signal));
                    PMUList.Add(signal.PMUname);
                    Data = Data.InsertColumn(Data.ColumnCount, signal.DataVector);
                }
            }
        }
        private string _typeConverter(string type)
        {
            switch (type)
            {
                case "VMP":
                case "VMA":
                case "VMB":
                case "VMC":
                    return "VPM";
                case "VAP":
                case "VAA":
                case "VAB":
                case "VAC":
                    return "VPA";
                case "IMP":
                case "IMA":
                case "IMB":
                case "IMC":
                    return "IPM";
                case "IAP":
                case "IAA":
                case "IAB":
                case "IAC":
                    return "IPA";
                default:
                    return type;
            }
        }
        private string _unitConverter(Signal sgl)
        {
            switch (sgl.Unit)
            {
                case "mHz":
                    sgl.DataVector = sgl.DataVector / 1000;
                    return "Hz";
                case "V":
                    sgl.DataVector = sgl.DataVector / 1000;
                    return "kV";
                case "RAD":
                    sgl.DataVector = sgl.DataVector * 180/ Math.PI;
                    return "DEG";
                default:
                    return sgl.Unit;
            }
        }
    }
}
