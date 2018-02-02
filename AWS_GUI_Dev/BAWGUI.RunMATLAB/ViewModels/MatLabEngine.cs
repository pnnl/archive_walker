using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using BAWSengine;
using BAWGUI.RunMATLAB.Models;

namespace BAWGUI.RunMATLAB.ViewModels
{
    public class MatLabEngine:ViewModelBase
    {
        private static readonly MatLabEngine _instance = new MatLabEngine();
        private bool _isMatlabEngineRunning;
        public bool IsMatlabEngineRunning
        {
            get { return _isMatlabEngineRunning; }
            set
            {
                _isMatlabEngineRunning = value;
                OnPropertyChanged();

            }
        }
        private string _controlPath;
        public string ControlPath
        {
            get { return _controlPath; }
            //set { _controlPath = value; }
        }
        private MatLabEngine()
        {
            _isMatlabEngineRunning = false;
            try
            {
                //_matlabEngine = new BAWSengine.GUI2MAT();
                _matlabEngine = new BAWSengine2.GUI2MAT();
            }
            catch (Exception)
            {
                
            }
        }
        public static MatLabEngine Instance
        {
            get
            {
                return _instance;
            }
        }
        //private BAWSengine.GUI2MAT _matlabEngine;
        private BAWSengine2.GUI2MAT _matlabEngine;
        public void RunNormalMode(string controlPath, string configFile)
        {
            _controlPath = controlPath;
            IsMatlabEngineRunning = true;
            _matlabEngine.RunNormalMode(controlPath, configFile);
            //TODO: ????????????maybe check if run flag exist, if yes, delete it.??????????????????
            IsMatlabEngineRunning = false;
        }
        public void RingDownRerun(string start, string end, string configFile)
        {
            int size = 0;
            int numberOfelements = 0;
            MWStructArray RingdownRerun = (MWStructArray)_matlabEngine.RerunRingdown(start, end, configFile);
            numberOfelements = RingdownRerun.NumberOfElements;
            for (int index = 1; index <= numberOfelements; index++)
            {
                Console.WriteLine("\nelement: " + index.ToString());
                MWNumericArray arr = (MWNumericArray)RingdownRerun["t", index];
                double[] t = (double[])(arr.ToVector(MWArrayComponent.Real));
                int[] dimEach = arr.Dimensions;
                Console.WriteLine("\tt array dimension is: " + dimEach[0].ToString() + " X " + dimEach[1].ToString());
                size = t.Length;
                //Console.WriteLine("first in t: " + _numbTimeConvert(t[0]) + " last in t: " + _numbTimeConvert(t[size - 1]));
                arr = (MWNumericArray)RingdownRerun["Data", index];
                double[] Data = (double[])arr.ToVector(MWArrayComponent.Real);
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    Console.WriteLine("matrix dimension out of range.");
                }
                size = dimEach[0];
                Console.WriteLine("\tdata array dimension is: " + dimEach[0].ToString() + " X " + dimEach[1].ToString());
                //for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                //{
                //    Console.WriteLine("\nChannel " + (signalCount + 1).ToString() + ": first in Data: " + Data[signalCount * dimEach[0]].ToString() + ", last in Data: " + Data[(signalCount + 1) * dimEach[0] - 1].ToString());
                //}
                arr = (MWNumericArray)RingdownRerun["RMS", index];
                double[] RMS = (double[])arr.ToVector(MWArrayComponent.Real);
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    Console.WriteLine("matrix dimension out of range.");
                }
                size = dimEach[0];
                Console.WriteLine("\tRMS array dimension is: " + dimEach[0].ToString() + " X " + dimEach[1].ToString());
                //for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                //{
                //    Console.WriteLine("\nChannel " + (signalCount + 1).ToString() + ": first in Data: " + RMS[signalCount * dimEach[0]].ToString() + ", last in Data: " + RMS[(signalCount + 1) * dimEach[0] - 1].ToString());
                //}
                arr = (MWNumericArray)RingdownRerun["Threshold", index];
                double[] Threshold = (double[])arr.ToVector(MWArrayComponent.Real);
                dimEach = arr.Dimensions;
                if (dimEach.Length != 2)
                {
                    Console.WriteLine("matrix dimension out of range.");
                }
                size = dimEach[0];
                Console.WriteLine("\tThreshold array dimension is: " + dimEach[0].ToString() + " X " + dimEach[1].ToString());
                //for (int signalCount = 0; signalCount < dimEach[1]; signalCount++)
                //{
                //    Console.WriteLine("\nChannel " + (signalCount + 1).ToString() + ": first in Data: " + Threshold[signalCount * dimEach[0]].ToString() + ", last in Data: " + Threshold[(signalCount + 1) * dimEach[0] - 1].ToString());
                //}
                List<string> DataPMU = new List<string>();
                foreach (char[,] item in ((MWCellArray)RingdownRerun["DataPMU", index]).ToArray())
                {
                    string pmu = "";
                    foreach (var c in item)
                    {
                        pmu = pmu + c.ToString();
                    }
                    DataPMU.Add(pmu);
                }
                Console.WriteLine("\tthere are " + DataPMU.Count + " PMUs");
                //Console.WriteLine("first PMU is: " + DataPMU.FirstOrDefault() + ", last PMU is: " + DataPMU.LastOrDefault());
                List<string> DataChannel = new List<string>();
                foreach (char[,] item in ((MWCellArray)RingdownRerun["DataChannel", index]).ToArray())
                {
                    string channel = "";
                    foreach (var c in item)
                    {
                        channel = channel + c.ToString();
                    }
                    DataChannel.Add(channel);
                }
                Console.WriteLine("\tthere are " + DataChannel.Count + " channels");
                //Console.WriteLine("first data channel is: " + DataChannel.FirstOrDefault() + ", last data channel is: " + DataChannel.LastOrDefault());
            }

        }
        public List<SparseDetector> GetSparseData(string start, string end, string configFilePath, string detector)
        {
            IsMatlabEngineRunning = true;
            var sparseR = new SparseResults((MWStructArray)_matlabEngine.GetSparseData(start, end, configFilePath, detector));
            IsMatlabEngineRunning = false;
            return sparseR.SparseDetectorList;
        }
        //private System.DateTime _numbTimeConvert(double item)
        //{
        //    System.DateTime dtDateTime = new DateTime(0001, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        //    System.DateTime bbb = dtDateTime.AddSeconds((item - 367) * 86400);
        //    return bbb;
        //}

    }
}
