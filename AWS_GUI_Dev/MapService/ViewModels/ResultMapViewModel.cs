using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Utilities;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using MapService.Models;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapService.ViewModels
{
    public class ResultMapViewModel : ViewModelBase
    {
        public ResultMapViewModel()
        {
            _curveDesignOption = CurveDesignOptions.Option1;
            _setupMap();
        }
        private List<SignalIntensityViewModel> _signals;
        public List<SignalIntensityViewModel> Signals
        {
            get { return _signals; }
            set
            {
                if (_signals != value)
                {
                    _signals = value;
                    _updateGmap();
                    OnPropertyChanged();
                }
            }
        }

        public int MaxZoom { get; set; } = 8;
        public int MinZoom { get; set; } = 0;
        private void _updateGmap()
        {
            var pointPairs = new List<PointsPair>();

            var signalToBePloted = Signals.Where(x => !double.IsNaN(x.Intensity)).ToList();

            double minColor = 0d, maxColor = 0d, colorRange = 0d;
            List<OxyColor> colors = null;
            var numberOfSigs = signalToBePloted.Count();
            if (numberOfSigs > 0)
            {
                minColor = signalToBePloted.Select(x => x.Intensity).Min();
                maxColor = signalToBePloted.Select(x => x.Intensity).Max();
                colorRange = maxColor - minColor;
                colors = OxyPalettes.Rainbow(numberOfSigs + 1).Colors.ToList();
            }
            SolidColorBrush color = null;
            foreach (var signal in signalToBePloted)
            {
                //if (!double.IsNaN(signal.Intensity))
                //{
                    if (colorRange != 0d)
                    {
                        var percentage = (int)Math.Round((signal.Intensity - minColor) / colorRange * numberOfSigs);
                        color = new SolidColorBrush(Color.FromArgb(colors[percentage].A, colors[percentage].R, colors[percentage].G, colors[percentage].B));
                    }
                    else
                    {
                        color = Brushes.Navy;
                    }
                    if (signal.Signal.MapPlotType == SignalMapPlotType.Dot)
                    {
                        //_addDotToMap();
                        var point = signal.Signal.Locations.FirstOrDefault();
                        if (double.TryParse(point.Latitude, out double la) && double.TryParse(point.Longitude, out double lg))
                        {
                            var mkr = new GMapMarker(new PointLatLng(la, lg));
                            mkr.Shape = new Ellipse
                            {
                                Width = 10,
                                Height = 10,
                                Stroke = color,
                                Fill = color,
                                ToolTip = signal.Signal.SignalName
                            };
                            mkr.Tag = signal.Signal.SignalName;
                            Gmap.Markers.Add(mkr);
                        }
                    }
                    if (signal.Signal.MapPlotType == SignalMapPlotType.Line)
                    {
                        //collect all point pairs here a list of tuples, maybe?
                        List<CartesianPoint> points = new List<CartesianPoint>();
                        for (int index = 0; index < signal.Signal.Locations.Count; index++)
                        {
                            if (double.TryParse(signal.Signal.Locations[index].Longitude, out double lg) && double.TryParse(signal.Signal.Locations[index].Latitude, out double la))
                            {
                                points.Add(new CartesianPoint(lg, la));
                            }
                        }
                        for (int index = 0; index < points.Count - 1; index++)
                        {
                            pointPairs.Add(new PointsPair(points[index], points[index + 1], color, signal.Signal.SignalName));
                        }
                    }
                    if (signal.Signal.MapPlotType == SignalMapPlotType.Area)
                    {
                        var points = new List<PointLatLng>();
                        var points2 = new List<Point>();
                        foreach (var pnt in signal.Signal.Locations)
                        {
                            if (double.TryParse(pnt.Latitude, out double la) && double.TryParse(pnt.Longitude, out double lg))
                            {
                                points.Add(new PointLatLng(la, lg));
                                points2.Add(new Point(la, lg));
                            }
                        }
                        var mkr = new GMap.NET.WindowsPresentation.GMapPolygon(points);
                        var areaColor = color.Clone();
                        areaColor.Opacity = 0.2;
                        mkr.Shape = new Path() { Stroke = color, StrokeThickness = 2, ToolTip = signal.Signal.SignalName, Fill = areaColor };
                        mkr.Tag = signal.Signal.SignalName;
                        Gmap.Markers.Add(mkr);
                        //_addPolygonToMap();
                    }
                //}
            }
            if (pointPairs.Count != 0)
            {
                try
                {
                    _drawCurvesOnMap(pointPairs);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error drawing arcs on map.\n" + ex.Message, "Error!", MessageBoxButtons.OK);
                }
            }
        }

        private void _drawCurvesOnMap(List<PointsPair> pointPairs)
        {
            //int numberOfPointsForEachCurve = 100;
            List<GMapRoute> curveList = _designCurves(pointPairs);
            foreach (var curve in curveList)
            {
                //TODO: change some properties of each curve so they look better
                Gmap.Markers.Add(curve);
            }
        }

        private List<GMapRoute> _designCurves(List<PointsPair> pointPairs)
        {
            List<GMapRoute> curveList = new List<GMapRoute>();
            var cuvreDistanceMatrix = new List<CurveDistanceHolder>();
            for (int index = 0; index < pointPairs.Count - 1; index++)
            {
                for (int index2 = index + 1; index2 < pointPairs.Count; index2++)
                {
                    cuvreDistanceMatrix.Add(new CurveDistanceHolder(pointPairs[index], pointPairs[index2]));
                }
            }
            var curveDistanceMatrixCopy = cuvreDistanceMatrix;
            CurveDistanceHolder selectedLinepair = null;
            while (cuvreDistanceMatrix.Count != 0)
            {
                if (CurveDesignOption == CurveDesignOptions.Option1)
                {
                    //cureDistanceMatrix.Select(x => x.MinDistance).Min();
                    selectedLinepair = cuvreDistanceMatrix.Aggregate((curMin, x) => x.MinDistance < curMin.MinDistance ? x : curMin);
                }
                else
                {
                    selectedLinepair = cuvreDistanceMatrix.Aggregate((curMax, x) => x.MaxDistanceDiff > curMax.MaxDistanceDiff ? x : curMax);
                }
                if (selectedLinepair != null)
                {
                    selectedLinepair.SetCurveSelection();
                    cuvreDistanceMatrix = cuvreDistanceMatrix.Where(x => x.PointPair1 != selectedLinepair.PointPair1 && x.PointPair2 != selectedLinepair.PointPair2 && x.PointPair1 != selectedLinepair.PointPair2 && x.PointPair2 != selectedLinepair.PointPair1).ToList();
                    //cureDistanceMatrix.RemoveAll(x => x.PointPair1 == selectedpair.PointPair1 || x.PointPair2 == selectedpair.PointPair2);
                    //curveList.Add(new GMapRoute(selectedpair.PointPair1.SelectedCurve);
                    //curveList.Add(selectedpair.PointPair2.SelectedCurve);
                }
                foreach (var pair in pointPairs)
                {
                    if (pair.SelectedCurve.Count == 0)
                    {
                        var foundSamePair = false;
                        PointsPair foundPair = null;
                        if ((pair.X1 == selectedLinepair.PointPair1.X1 && pair.Y1 == selectedLinepair.PointPair1.Y1 && pair.X2 == selectedLinepair.PointPair1.X2 && pair.Y2 == selectedLinepair.PointPair1.Y2)
                        || (pair.X1 == selectedLinepair.PointPair1.X2 && pair.Y1 == selectedLinepair.PointPair1.Y2 && pair.X2 == selectedLinepair.PointPair1.X1 && pair.Y2 == selectedLinepair.PointPair1.Y1))
                        {
                            foundSamePair = true;
                            foundPair = selectedLinepair.PointPair1;
                        }
                        if ((pair.X1 == selectedLinepair.PointPair2.X1 && pair.Y1 == selectedLinepair.PointPair2.Y1 && pair.X2 == selectedLinepair.PointPair2.X2 && pair.Y2 == selectedLinepair.PointPair2.Y2)
                        || (pair.X1 == selectedLinepair.PointPair2.X2 && pair.Y1 == selectedLinepair.PointPair2.Y2 && pair.X2 == selectedLinepair.PointPair2.X1 && pair.Y2 == selectedLinepair.PointPair2.Y1))
                        {
                            if (foundSamePair)
                            {
                                throw new Exception("A maximum of two lines can be drawn between two points."); //TODO: Or we can change the arc of the theta of this pair, if we want to draw unlimited arcs between 2 points
                            }
                            else
                            {
                                foundSamePair = true;
                                foundPair = selectedLinepair.PointPair1;
                            }
                        }
                        if (foundSamePair && foundPair != null)
                        {
                            if (pair.Center1.Distance(foundPair.SelectedCenter) < pair.Center2.Distance(foundPair.SelectedCenter))
                            {
                                pair.SelectedCenter = pair.Center2;
                                pair.SelectedCurve = pair.Curve2;
                            }
                            else
                            {
                                pair.SelectedCenter = pair.Center1;
                                pair.SelectedCurve = pair.Curve1;
                            }
                        }
                        if (pair.SelectedCurve.Count != 0)
                        {
                            cuvreDistanceMatrix = cuvreDistanceMatrix.Where(x => x.PointPair1 != pair && x.PointPair2 != pair).ToList();
                            //cureDistanceMatrix.RemoveAll(x => x.PointPair1 == pair || x.PointPair2 == pair);
                        }
                    }
                }
            }
            var remainedPair = pointPairs.Find(x => x.SelectedCurve.Count == 0);
            if (remainedPair != null)
            {
                var pairedWithRemainedPair = curveDistanceMatrixCopy.Where(x => x.PointPair1 == remainedPair || x.PointPair2 == remainedPair).ToList();
                if (pairedWithRemainedPair.Count != 0)
                {
                    var nearestNeighbor = pairedWithRemainedPair.Aggregate((curMin, x) => x.MinDistance < curMin.MinDistance ? x : curMin);
                    if (remainedPair == nearestNeighbor.PointPair1)
                    {
                        if (nearestNeighbor.PointPair2.SelectedCenter == nearestNeighbor.PointPair2.Center1)
                        {
                            if (nearestNeighbor.Distance11 > nearestNeighbor.Distance21)
                            {
                                remainedPair.SelectedCenter = remainedPair.Center1;
                                remainedPair.SelectedCurve = remainedPair.Curve1;
                            }
                            else
                            {
                                remainedPair.SelectedCenter = remainedPair.Center2;
                                remainedPair.SelectedCurve = remainedPair.Curve2;
                            }
                        }
                        else
                        {
                            if (nearestNeighbor.Distance12 > nearestNeighbor.Distance22)
                            {
                                remainedPair.SelectedCenter = remainedPair.Center1;
                                remainedPair.SelectedCurve = remainedPair.Curve1;
                            }
                            else
                            {
                                remainedPair.SelectedCenter = remainedPair.Center2;
                                remainedPair.SelectedCurve = remainedPair.Curve2;
                            }
                        }
                    }
                    else
                    {
                        if (nearestNeighbor.PointPair1.SelectedCenter == nearestNeighbor.PointPair1.Center1)
                        {
                            if (nearestNeighbor.Distance11 > nearestNeighbor.Distance12)
                            {
                                remainedPair.SelectedCenter = remainedPair.Center1;
                                remainedPair.SelectedCurve = remainedPair.Curve1;
                            }
                            else
                            {
                                remainedPair.SelectedCenter = remainedPair.Center2;
                                remainedPair.SelectedCurve = remainedPair.Curve2;
                            }
                        }
                        else
                        {
                            if (nearestNeighbor.Distance21 > nearestNeighbor.Distance22)
                            {
                                remainedPair.SelectedCenter = remainedPair.Center1;
                                remainedPair.SelectedCurve = remainedPair.Curve1;
                            }
                            else
                            {
                                remainedPair.SelectedCenter = remainedPair.Center2;
                                remainedPair.SelectedCurve = remainedPair.Curve2;
                            }
                        }
                    }
                }
                else
                {
                    remainedPair.SelectedCenter = remainedPair.Center1;
                    remainedPair.SelectedCurve = remainedPair.Curve1;
                }
            }
            foreach (var pair in pointPairs)
            {
                var newCurve = new List<PointLatLng>();
                foreach (var p in pair.SelectedCurve)
                {
                    newCurve.Add(new PointLatLng(p.Y, p.X));
                }
                var newRoute = new GMapRoute(newCurve);
                newRoute.Shape = new Path() { Stroke = pair.Color, StrokeThickness = 2, ToolTip = pair.Tag };
                newRoute.Tag = pair.Tag;
                curveList.Add(newRoute);
            }
            return curveList;
        }

        public void ClearMarkers()
        {
            Gmap.Markers.Clear();
        }

        //private double _distanceBetween2Curves(List<CartesianPoint> curve1, List<CartesianPoint> curve2)
        //{
        //    var xAccDiff = 0d;
        //    var yAccDiff = 0d;
        //    var xAccDiffRev = 0d;
        //    var yAccDiffRev = 0d;
        //    for (int index = 0; index < curve1.Count; index++)
        //    {
        //        xAccDiff = xAccDiff + Math.Pow(curve1[index].X - curve2[index].X, 2);
        //        yAccDiff = yAccDiff + Math.Pow(curve1[index].Y - curve2[index].Y, 2);
        //        xAccDiffRev = xAccDiffRev + Math.Pow(curve1[index].X - curve2[curve1.Count - index].X, 2);
        //        yAccDiffRev = yAccDiffRev + Math.Pow(curve1[index].Y - curve2[curve1.Count - index].Y, 2);
        //    }
        //    var d1 = Math.Sqrt((xAccDiff + yAccDiff) / curve1.Count);
        //    var d2 = Math.Sqrt((xAccDiffRev + yAccDiffRev) / curve1.Count);
        //    return Math.Min(d1, d2);
        //}
        private CurveDesignOptions _curveDesignOption;
        public CurveDesignOptions CurveDesignOption
        {
            get { return _curveDesignOption; }
            set
            {
                _curveDesignOption = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// input should be a list of tuples of point pairs?
        /// </summary>
        private void _addLineToMap()
        {
            //query jim's function to get curves first?
            //Then draw the curves on map
        }

        private void _addPolygonToMap()
        {
        }

        private void _addDotToMap()
        {
            //throw new NotImplementedException();
        }

        private void _setupMap()
        {
            Gmap = new GMapControl();
            Gmap.MaxZoom = MaxZoom;
            Gmap.MinZoom = MinZoom;
            Gmap.Zoom = 5;
            Gmap.CenterPosition = new PointLatLng(37.0902, -95.7129);
            Gmap.ShowCenter = false;
            Gmap.MapProvider = GMapProviders.OpenStreetMap;
            Gmap.Manager.Mode = AccessMode.CacheOnly;
            Gmap.CacheLocation = "..\\MapCache";
        }

        private GMapControl _gMap;
        public GMapControl Gmap
        {
            get { return _gMap; }
            set
            {
                _gMap = value;
                OnPropertyChanged();
            }
        }

        public void UpdateResultMap(List<SignalIntensityViewModel> signalList)
        {
            Signals = signalList;
        }
        public void AddLineTest()
        {
            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(40, -111));
            points.Add(new PointLatLng(30, -100));
            points.Add(new PointLatLng(35, -90));
            GMapRoute mRoute = new GMapRoute(points);
            //mRoute.ZIndex = 1;
            Gmap.Markers.Add(mRoute);
            //GMapPolygon polygon = new GMapPolygon(points);
            //Gmap.Markers.Add(polygon);
        }

        //private OxyPalettes _rbColors = OxyPalettes.Rainbow(100);
        //private OxyColor _mapFrequencyToColor(float frequency)
        //{
        //    //OxyColor color;
        //    var colorCount = FilteredResults.Count;
        //    var minFreq = FilteredResults.Select(x => x.TypicalFrequency).Min() - 0.1;
        //    var maxFreq = FilteredResults.Select(x => x.TypicalFrequency).Max() + 0.1;
        //    var percentage = (frequency - minFreq) / (maxFreq - minFreq);

        //    //blue-green rgb gradient
        //    return OxyColor.FromRgb(0, Convert.ToByte(255 * percentage), Convert.ToByte(255 * (1 - percentage)));

        //    //blue-purple-red gradient
        //    //return OxyColor.FromRgb(Convert.ToByte(255 * percentage), 0, Convert.ToByte(255 * (1 - percentage)));

        //    //blue-white-red gradient
        //    //if (percentage < 0.5)
        //    //{
        //    //    return OxyColor.FromRgb(Convert.ToByte(255 * percentage), Convert.ToByte(255 * percentage), 255);
        //    //}
        //    //else
        //    //{
        //    //    return OxyColor.FromRgb(255, Convert.ToByte(255 * (1 - percentage)), Convert.ToByte(255 * (1 - percentage)));
        //    //}

        //    //blue-white-green gradient
        //    //if (percentage < 0.5)
        //    //{
        //    //    return OxyColor.FromRgb(Convert.ToByte(255 * percentage), Convert.ToByte(255 * percentage), 255);
        //    //}
        //    //else
        //    //{
        //    //    return OxyColor.FromRgb(Convert.ToByte(255 * (1 - percentage)), 255, Convert.ToByte(255 * (1 - percentage)));
        //    //}
        //}

    }
}
