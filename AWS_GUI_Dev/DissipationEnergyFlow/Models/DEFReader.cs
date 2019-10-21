using BAWGUI.CoordinateMapping.Models;
using BAWGUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DissipationEnergyFlow.Models
{
    public class DEFReader
    {
        private XElement _item;
        private DissipationEnergyFlowDetectorModel _detector;
        public DEFReader()
        {
            _detector = new DissipationEnergyFlowDetectorModel();
        }
        public DEFReader(XElement item) : this()
        {
            _readDEFDetectorConfig(item);
        }

        private void _readDEFDetectorConfig(XElement item)
        {
            this._item = item;
            var paths = _item.Element("Paths");
            if (paths != null)
            {
                var pths = paths.Elements("Path");
                if (pths != null)
                {
                    var newPaths = new List<EnergyFlowPath>();
                    foreach (var pth in pths)
                    {
                        var newPath = new EnergyFlowPath();
                        var fromArea = pth.Element("From");
                        if (fromArea != null)
                        {
                            var thisAreaName = fromArea.Value;
                            //check to see if this newly read area name is in the DEF model area dictionary
                            //if not, assign this name to the area objct in the path and add this object to the DEF model area dictionary
                            //if yes, re-point the area object in the DEF path to the existing area object in the dictionary
                            newPath.FromArea = thisAreaName;
                            if (!_detector.Areas.ContainsKey(thisAreaName))
                            {
                                _detector.Areas[thisAreaName] = new EnergyFlowAreaCoordsMappingModel(thisAreaName);
                            }
                            //else
                            //{
                            //    newPath.FromArea = _detector.Areas[thisAreaName];
                            //}
                        }
                        var toArea = pth.Element("To");
                        if (toArea != null)
                        {
                            var thisAreaName = toArea.Value;
                            //check to see if this newly read area name is in the DEF model area dictionary
                            //if not, assign this name to the area objct in the path and add this object to the DEF model area dictionary
                            //if yes, re-point the area object in the DEF path to the existing area object in the dictionary
                            newPath.ToArea = thisAreaName;
                            if (!_detector.Areas.ContainsKey(thisAreaName))
                            {
                                _detector.Areas[thisAreaName] = new EnergyFlowAreaCoordsMappingModel(thisAreaName);
                            }
                            //else
                            //{
                            //    newPath.ToArea = _detector.Areas[thisAreaName];
                            //}
                        }
                        var vm = pth.Element("VM");
                        if (vm != null)
                        {
                            newPath.VoltageMag = _readPathSignal(newPath, vm);
                        }
                        var va = pth.Element("VA");
                        if (va != null)
                        {
                            newPath.VoltageAng = _readPathSignal(newPath, va);
                        }
                        var p = pth.Element("P");
                        if (p != null)
                        {
                            newPath.ActivePowerP = _readPathSignal(newPath, p);
                        }
                        var q = pth.Element("Q");
                        if (q != null)
                        {
                            newPath.ReactivePowerQ = _readPathSignal(newPath, q);
                        }
                        newPaths.Add(newPath);
                    }
                    _detector.Paths = newPaths;
                }
            }
            var parameters = _item.Element("Parameters");
            if (parameters != null)
            {
                var par = parameters.Element("LocMinLength");
                if (par != null)
                {
                    try
                    {
                        _detector.LocMinLength = Int32.Parse(par.Value);
                    }
                    catch (Exception ex)
                    {

                        throw new Exception("Integer expected. Original error: " + ex.Message);
                    }
                }
                par = parameters.Element("LocLengthStep");
                if (par != null)
                {
                    try
                    {
                        _detector.LocLengthStep = Int32.Parse(par.Value);
                    }
                    catch (Exception ex)
                    {

                        throw new Exception("Integer expected. Original error: " + ex.Message);
                    }
                }
                par = parameters.Element("LocRes");
                if (par != null)
                {
                    try
                    {
                        _detector.LocRes = Int32.Parse(par.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Integer expected. Original error: " + ex.Message);
                    }
                }
            }
        }
        private static SignalSignatures _readPathSignal(EnergyFlowPath newPath, XElement el)
        {
            SignalSignatures sig = null;
            var pmu = el.Element("PMU");
            if (pmu != null)
            {
                var name = pmu.Element("Name");
                var channel = pmu.Element("Channel");
                if (channel != null)
                {
                    var chName = channel.Element("Name");
                    if (chName != null || name != null)
                    {
                        sig = new SignalSignatures(name.Value, chName.Value);
                    }
                }
            }
            return sig;
        }
        public DissipationEnergyFlowDetectorModel ReadDEFDetectorConfig(XElement item)
        {
            _readDEFDetectorConfig(item);
            return _detector;
        }
        public DissipationEnergyFlowDetectorModel GetDEFDetector()
        {
            return _detector;
        }
    }
}
