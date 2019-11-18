using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Core.Utilities;
using BAWGUI.Utilities;
using BAWGUI.Xml;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using MapService.Models;
using MapService.Views;
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
            _rbcolors = new List<OxyColor>();
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
        private List<OxyColor> _rbcolors;
        public List<OxyColor> RBColors
        {
            get { return _rbcolors; }
            set
            {
                _rbcolors = value;
                OnPropertyChanged();
            }
        }
        private double _maxColor = 0d;
        public double MaxColor
        {
            get { return _maxColor; }
            set
            {
                _maxColor = value;
                OnPropertyChanged();
            }
        }
        private double _minColor = 0d;
        public double MinColor
        {
            get { return _minColor; }
            set
            {
                _minColor = value;
                OnPropertyChanged();
            }
        }
        private void _updateGmap()
        {
            var pointPairs = new List<PointsPair>();

            var signalToBePloted = Signals.Where(x => !double.IsNaN(x.Intensity)).ToList();
            signalToBePloted = _cleanUnassignedSites(signalToBePloted);

            double colorRange = 0d;
            //List<OxyColor> RBColors = null;
            var numberOfSigs = signalToBePloted.Count();
            if (numberOfSigs > 0)
            {
                MinColor = signalToBePloted.Select(x => x.Intensity).Min();
                MaxColor = signalToBePloted.Select(x => x.Intensity).Max();
                colorRange = MaxColor - MinColor;
            }
            if (numberOfSigs > 1)
            {
                RBColors = OxyPalettes.Jet(numberOfSigs).Colors.ToList();
            }
            else
            {
                RBColors = new List<OxyColor>() { OxyColors.Navy };
                //RBColors.Add(OxyColors.Navy);
            }
            SolidColorBrush color = null;
            foreach (var signal in signalToBePloted)
            {
                //if (!double.IsNaN(signal.Intensity))
                //{
                if (colorRange != 0d)
                {
                    var percentage = (int)Math.Round((signal.Intensity - MinColor) / colorRange * (numberOfSigs - 1));
                    color = new SolidColorBrush(Color.FromArgb(RBColors[percentage].A, RBColors[percentage].R, RBColors[percentage].G, RBColors[percentage].B));
                }
                else
                {
                    color = new SolidColorBrush(Color.FromArgb(RBColors[0].A, RBColors[0].R, RBColors[0].G, RBColors[0].B));
                }
                if (signal.Signal.MapPlotType == SignalMapPlotType.Dot)
                {
                    var point = signal.Signal.Locations.FirstOrDefault();
                    if (double.TryParse(point.Latitude, out double la) && double.TryParse(point.Longitude, out double lg))
                    {
                        var mkr = new GMapMarker(new PointLatLng(la, lg));
                        mkr.Shape = new Ellipse
                        {
                            Width = 15,
                            Height = 15,
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
                    //collect all point pairs here if there's only 2 points in the line
                    //if there's more than 2 points, draw straight line.
                    List<CartesianPoint> points = new List<CartesianPoint>();
                    var newLine = new List<PointLatLng>();
                    for (int index = 0; index < signal.Signal.Locations.Count; index++)
                    {
                        if (double.TryParse(signal.Signal.Locations[index].Longitude, out double lg) && double.TryParse(signal.Signal.Locations[index].Latitude, out double la))
                        {
                            points.Add(new CartesianPoint(lg, la));
                            newLine.Add(new PointLatLng(la, lg));
                        }
                    }
                    if (points.Count > 2)
                    {
                        //to plot them straight
                        var newRoute = new GMapRoute(newLine);
                        newRoute.Shape = new Path() { Stroke = color, StrokeThickness = 4, ToolTip = signal.Signal.SignalName };
                        newRoute.Tag = signal.Signal.SignalName;
                        Gmap.Markers.Add(newRoute);

                        //to plot them as curves
                        //for (int i = 0; i < points.Count - 1; i++)
                        //{
                        //    pointPairs.Add(new PointsPair(points[i], points[i+1], color, signal.Signal.SignalName));
                        //}
                    }
                    else if (points.Count == 2)
                    {
                        pointPairs.Add(new PointsPair(points[0], points[1], color, signal.Signal.SignalName));
                    }
                    else
                    {
                        throw new Exception("A signal line should have at least 2 points/locations specified.");
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

        private List<SignalIntensityViewModel> _cleanUnassignedSites(List<SignalIntensityViewModel> signalToBePloted)
        {
            var rlt = new List<SignalIntensityViewModel>();
            foreach (var item in signalToBePloted)
            {
                for (int i = item.Signal.Locations.Count - 1; i >= 0; i--)
                {
                    if (item.Signal.Locations[i] == CoreUtilities.DummySiteCoordinatesModel)
                    {
                        item.Signal.Locations.RemoveAt(i);
                    }
                }
                if (item.Signal.Locations.Count != 0)
                {
                    rlt.Add(item);
                }
            }
            return rlt;
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
                newRoute.Shape = new Path() { Stroke = pair.Color, StrokeThickness = 4, ToolTip = pair.Tag };
                newRoute.Tag = pair.Tag;
                curveList.Add(newRoute);
            }
            return curveList;
        }

        public void DrawDEF(Dictionary<string, Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>> entries, List<ForcedOscillationTypeOccurrencePath> paths)
        {
            var RBColors = OxyPalettes.Jet(entries.Count() + 1).Colors.ToList();
            foreach (var color in RBColors)
            {
                if (color.Equals(OxyColors.Red))
                {
                    RBColors.Remove(color);
                    break;
                }
            }
            int areaCount = 0;
            Gmap.Markers.Clear();
            var drawnArea = new Dictionary<string, bool>();
            entries = _cleanDEFUnassignedSites(entries);
            //var sortedPath = paths.OrderByDescending(x => Math.Abs(x.DEF)).ToList();
            //var thinkest = Math.Abs(sortedPath.FirstOrDefault().DEF);
            var sortedPath = paths.OrderBy(x => Math.Abs(x.DEF)).ToList();
            var thinnest = Math.Abs(sortedPath.FirstOrDefault().DEF);//actual thinnest
            var thickest = Math.Abs(sortedPath.LastOrDefault().DEF);//actual thinnest
            foreach (var pth in paths)
            {
                if (!drawnArea.ContainsKey(pth.From) && entries.ContainsKey(pth.From))
                {
                    areaCount = _drawDEFArea(entries, RBColors, areaCount, drawnArea, pth.From);
                }
                if (!drawnArea.ContainsKey(pth.To) && entries.ContainsKey(pth.To))
                {
                    areaCount = _drawDEFArea(entries, RBColors, areaCount, drawnArea, pth.To);
                }
                if (entries.ContainsKey(pth.To) && entries.ContainsKey(pth.From))
                {
                    _drawArrow(entries, pth, thinnest, thickest, drawnArea);
                }
                else if (entries.ContainsKey(pth.To) && !entries.ContainsKey(pth.From))
                {
                    _drawFixedArrow(entries, pth, thinnest, thickest, true, drawnArea);
                }
                else if (!entries.ContainsKey(pth.To) && entries.ContainsKey(pth.From))
                {
                    _drawFixedArrow(entries, pth, thinnest, thickest, false, drawnArea);
                }
            }
            foreach (var mkr in Gmap.Markers)
            {
                var name = mkr.Tag.ToString();
                if (drawnArea.ContainsKey(name) && drawnArea[name])
                {
                    if (entries[name].Item1 == SignalMapPlotType.Dot)
                    {
                        var s = mkr.Shape as Ellipse;
                        s.Stroke = Brushes.Red;
                        s.Fill = Brushes.Red;
                    }
                    if (entries[name].Item1 == SignalMapPlotType.Line)
                    {
                        var s = mkr.Shape as Path;
                        s.Stroke = Brushes.Red;
                    }
                    if (entries[name].Item1 == SignalMapPlotType.Area)
                    {
                        var s = mkr.Shape as Path;
                        s.Stroke = Brushes.Red;
                        var areaColor = Brushes.Red.Clone();
                        areaColor.Opacity = 0.2;
                        s.Fill = areaColor;
                    }

                }
            }
            Gmap.InvalidateVisual(false);
            //Gmap.UpdateLayout();
        }
        //when of the area in the path is not defined or not available in the SiteCoordinatesModel dictionary. As from Jim: When displaying DEF on the map, can you make a special case for when the To area is empty? In this case the From area could have a horizontal or vertical arrow pointing to/from it. 
        private void _drawFixedArrow(Dictionary<string, Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>> entries, ForcedOscillationTypeOccurrencePath pth, float thinnest, float thickest, bool noFrom, Dictionary<string, bool> drawnArea)
        {
            PointLatLng existingCenter = new PointLatLng();
            SignalMapPlotType existingCenterType;
            string pathLabel = "";
            string arrowHeadLabel = "";
            bool toExistingCenter = true;
            if (noFrom)
            {
                existingCenter = _getAreaCenter(entries[pth.To].Item2);
                existingCenterType = entries[pth.To].Item1;
                if (pth.DEF > 0)
                {
                    pathLabel = "Unknown Area to " + pth.To;
                    arrowHeadLabel = pth.To + "_arrow_head";
                    toExistingCenter = true;
                    drawnArea[pth.To] = false;
                }
                else
                {
                    pathLabel = pth.To + " to Unknown Area";
                    arrowHeadLabel = "Unknown Area";
                    toExistingCenter = false;
                }
            }
            else
            {
                existingCenter = _getAreaCenter(entries[pth.From].Item2);
                existingCenterType = entries[pth.From].Item1;
                if (pth.DEF > 0)
                {
                    pathLabel = pth.From + " to Unkonwn Area";
                    arrowHeadLabel = "Unknown Area";
                    toExistingCenter = false;
                }
                else
                {
                    pathLabel = "Unkonwn Area to " + pth.From;
                    arrowHeadLabel = pth.From + "_arrow_head";
                    toExistingCenter = true;
                    drawnArea[pth.From] = false;
                }
            }
            var relativeEFstrength = (int)Math.Round((Math.Abs(pth.DEF) - thinnest) / (thickest - thinnest) * 16, MidpointRounding.AwayFromZero) + 4;
            var arrowHeadSize = relativeEFstrength * 1.5;


            var sp = Gmap.FromLatLngToLocal(existingCenter);
            var pointsCollection = new PointCollection();
            double lineStart = 0, lineEnd = 0;
            if (toExistingCenter)
            {
                pointsCollection.Add(new Point(10, 0));
                pointsCollection.Add(new Point(Math.Cos(Math.PI / 5) * arrowHeadSize + 10, Math.Sin(Math.PI / 5) * arrowHeadSize));
                pointsCollection.Add(new Point(Math.Cos(Math.PI / 5) * arrowHeadSize + 10, - Math.Sin(Math.PI / 5) * arrowHeadSize));
                lineStart = Math.Cos(Math.PI / 5) * arrowHeadSize + 10;
                lineEnd = 100;
            }
            else
            {
                pointsCollection.Add(new Point(100, 0));
                pointsCollection.Add(new Point(100 - Math.Cos(Math.PI / 5) * arrowHeadSize, Math.Sin(Math.PI / 5) * arrowHeadSize));
                pointsCollection.Add(new Point(100 - Math.Cos(Math.PI / 5) * arrowHeadSize, -Math.Sin(Math.PI / 5) * arrowHeadSize));
                lineStart = 10;
                lineEnd = 100 - Math.Cos(Math.PI / 5) * arrowHeadSize;
            }

            // arrow head
            var mkr = new GMapMarker(existingCenter);
            mkr.Shape = new Polygon
            {
                Stroke = Brushes.Black,
                Fill = Brushes.Black,
                ToolTip = arrowHeadLabel,
                Points = pointsCollection
            };
            mkr.Tag = arrowHeadLabel;
            Gmap.Markers.Add(mkr);
            mkr.ZIndex = 0;


            //body of arrow
            var mkr2 = new GMapMarker(existingCenter);
            mkr2.Shape = new Line
            {
                Stroke = Brushes.Black,
                Fill = Brushes.Black,
                ToolTip = pathLabel,
                X1 = lineStart,
                Y1 = 0,
                X2 = lineEnd,
                Y2 = 0,
                StrokeThickness = relativeEFstrength
            };
            mkr2.Tag = pathLabel;
            Gmap.Markers.Add(mkr2);
            mkr2.ZIndex = 0;


        }

        private Dictionary<string, Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>> _cleanDEFUnassignedSites(Dictionary<string, Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>> entries)
        {
            var rlt = new Dictionary<string, Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>>();
            foreach (var item in entries)
            {
                var key = item.Key;
                var type = item.Value.Item1;
                for (int i = item.Value.Item2.Count - 1; i >= 0; i--)
                {
                    if (item.Value.Item2[i] == CoreUtilities.DummySiteCoordinatesModel)
                    {
                        item.Value.Item2.RemoveAt(i);
                    }
                }
                if (item.Value.Item2.Count != 0)
                {
                    rlt[key] = new Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>(type, item.Value.Item2);
                }
            }
            return rlt;
        }

        private void _drawArrow(Dictionary<string, Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>> entries, ForcedOscillationTypeOccurrencePath pth, float thinnest, float thickest, Dictionary<string, bool> drawnArea)
        {
            var relativeEFstrength = (int)Math.Round((Math.Abs(pth.DEF) - thinnest) / (thickest - thinnest) * 16, MidpointRounding.AwayFromZero) + 4;
            var arrowHeadSize = relativeEFstrength * 1.5;

            PointLatLng fromAreaCenter = new PointLatLng();
            PointLatLng toAreaCenter = new PointLatLng();
            //SignalMapPlotType toCenterType;
            string pathLabel = "";
            string arrowHeadLabel = "";
            if (pth.DEF > 0)
            {
                fromAreaCenter = _getAreaCenter(entries[pth.From].Item2);
                toAreaCenter = _getAreaCenter(entries[pth.To].Item2);
                //toCenterType = entries[pth.To].Item1;
                pathLabel = pth.From + " to " + pth.To;
                arrowHeadLabel = pth.To + "_arrow_head";
                drawnArea[pth.To] = false;
            }
            else
            {
                fromAreaCenter = _getAreaCenter(entries[pth.To].Item2);
                toAreaCenter = _getAreaCenter(entries[pth.From].Item2);
                //toCenterType = entries[pth.From].Item1;
                pathLabel = pth.To + " to " + pth.From;
                arrowHeadLabel = pth.From + "_arrow_head";
                drawnArea[pth.From] = false;
            }
            //var relativeEFstrength = (int)(Math.Abs(Math.Log10(Math.Abs(pth.DEF)) / Math.Log10(thinkest)) * 10);
            //var relativeEFstrength = (int)(Math.Abs(Math.Log10(Math.Abs(pth.DEF) / thinnest)) * 2) + 4;
            double upperx = 0d, lowerx = 0d, uppery = 0d, lowery = 0d, slope = 0d;
            var tosp = Gmap.FromLatLngToLocal(toAreaCenter);
            var frsp = Gmap.FromLatLngToLocal(fromAreaCenter);
            //var xDiff = fromAreaCenter.Lng - toAreaCenter.Lng;
            //var yDiff = fromAreaCenter.Lat - toAreaCenter.Lat;
            var xDiff = tosp.X - frsp.X;
            var yDiff = tosp.Y - frsp.Y;
            if (xDiff != 0)
            {
                slope = -(double)yDiff / (double)xDiff; // as y axis in screen coordinate system point to downward, the slope has to be negated to match the cartesian coordiate system.
            }
            else
            {
                if (yDiff >= 0)
                {
                    slope = double.PositiveInfinity;
                }
                else
                {
                    slope = double.NegativeInfinity;
                }
            }
            var upperAngle = Math.Atan(slope) + Math.PI / 5;
            var lowerAngle = Math.Atan(slope) - Math.PI / 5;
            //Console.WriteLine("path: {0}", pathLabel);
            //Console.WriteLine("angles: arrow body: {2} upper: {0}, lower: {1}", upperAngle * 180 / Math.PI, lowerAngle * 180 / Math.PI, Math.Atan(slope) * 180 / Math.PI);

            var pointsCollection = new PointCollection();
            //Point markerOffset = new Point(0, 0);
            var markerOffsetX = Math.Cos(Math.Atan(slope)) * 15;
            var markerOffsetY = Math.Sin(Math.Atan(slope)) * 15;
            //if (toCenterType == SignalMapPlotType.Dot)
            //{
            //    if (fromAreaCenter.Lng >= toAreaCenter.Lng)
            //    {
            //        //from location is in the first or 4th quadrant of to location
            //        markerOffset = new Point(markerOffsetX, -markerOffsetY);
            //    }
            //    else
            //    {
            //        //from location is in the 2nd or 3rd quadrant of to location
            //        markerOffset = new Point(-markerOffsetX, markerOffsetY);
            //    }
            //}
            //Console.WriteLine("screen position: x: {0}, y: {1}", sp.X, sp.Y);
            if (fromAreaCenter.Lng >= toAreaCenter.Lng)
            {
                upperx = Math.Cos(upperAngle) * arrowHeadSize + markerOffsetX;
                uppery = -Math.Sin(upperAngle) * arrowHeadSize - markerOffsetY;
                lowerx = Math.Cos(lowerAngle) * arrowHeadSize + markerOffsetX;
                lowery = -Math.Sin(lowerAngle) * arrowHeadSize - markerOffsetY;
                pointsCollection.Add(new Point(markerOffsetX, -markerOffsetY));
            }
            else
            {
                upperx = -Math.Cos(upperAngle) * arrowHeadSize - markerOffsetX;
                uppery = Math.Sin(upperAngle) * arrowHeadSize + markerOffsetY;
                lowerx = -Math.Cos(lowerAngle) * arrowHeadSize - markerOffsetX;
                lowery = Math.Sin(lowerAngle) * arrowHeadSize + markerOffsetY;
                pointsCollection.Add(new Point(-markerOffsetX, markerOffsetY));
            }
            pointsCollection.Add(new Point(upperx, uppery));
            pointsCollection.Add(new Point(lowerx, lowery));
            //Console.WriteLine("upper screen position: x: {0}, y: {1}", upperx, uppery);
            //Console.WriteLine("lower screen position: x: {0}, y: {1}", lowerx, lowery);

            ////pointsCollection.Add(new Point(sp.X - sp.X, sp.Y - sp.Y));
            ////pointsCollection.Add(new Point(upperx - sp.X, uppery - sp.Y));
            ////pointsCollection.Add(new Point(lowerx - sp.X, lowery - sp.Y));
            //var newOffsetx = (upperx + lowerx) / 2;
            //var newOffsety = (uppery + lowery) / 2;
            //pointsCollection.Add(new Point(sp.X - newOffsetx, sp.Y - newOffsety));
            //pointsCollection.Add(new Point(upperx - newOffsetx, uppery - newOffsety));
            //pointsCollection.Add(new Point(lowerx - newOffsetx, lowery - newOffsety));


            var mkr = new GMapMarker(toAreaCenter);
            mkr.Shape = new Polygon
            {
                Stroke = Brushes.Black,
                Fill = Brushes.Black,
                Points = pointsCollection,
                ToolTip = arrowHeadLabel,
            };
            mkr.Tag = arrowHeadLabel;
            //mkr.Offset = markerOffset;
            Gmap.Markers.Add(mkr);

            //RotateTransform rotation = mkr.Shape.RenderTransform as RotateTransform;
            //if (rotation != null) // Make sure the transform is actually a RotateTransform
            //{
            //    double rotationInDegrees = rotation.Angle;
            //    // Do something with the rotationInDegrees here, if needed...
            //}


            //var upperlatlng = Gmap.FromLocalToLatLng((int)upperx, (int)uppery);
            //var lowerlatlng = Gmap.FromLocalToLatLng((int)lowerx, (int)lowery);
            //var newline = new List<PointLatLng>();
            //newline.Add(fromAreaCenter);
            //newline.Add(toAreaCenter);
            //var newRoute = new GMapRoute(newline);
            //newRoute.Shape = new Path() { StrokeThickness = relativeEFstrength, ToolTip = pathLabel, Stroke = Brushes.Black, StrokeEndLineCap = PenLineCap.Triangle };
            //newRoute.Tag = pathLabel;
            ////newRoute.Offset = markerOffset;
            //Gmap.Markers.Add(newRoute);
            ////Gmap.RegenerateShape(newRoute);


            double x1, x2, y1, y2;
            x1 = (upperx + lowerx) / 2;
            y1 = (uppery + lowery) / 2;

            if (fromAreaCenter.Lng >= toAreaCenter.Lng)
            {
                x2 = frsp.X - tosp.X - markerOffsetX;
                y2 = frsp.Y - tosp.Y + markerOffsetY;
            }
            else
            {
                x2 = frsp.X - tosp.X + markerOffsetX;
                y2 = frsp.Y - tosp.Y - markerOffsetY;
            }
            var mkr2 = new GMapMarker(toAreaCenter);
            mkr2.Shape = new Line
            {
                Stroke = Brushes.Black,
                Fill = Brushes.Black,
                ToolTip = pathLabel,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                StrokeThickness = relativeEFstrength
            };
            mkr2.Tag = new Tuple<PointLatLng, PointLatLng>(toAreaCenter, fromAreaCenter);
            Gmap.Markers.Add(mkr2);
        }

        private PointLatLng _getAreaCenter(List<SiteCoordinatesModel> locations)
        {
            var lng = locations.Average(x => double.Parse(x.Longitude));
            var lat = locations.Average(x => double.Parse(x.Latitude));
            return new PointLatLng(lat, lng);
        }

        private int _drawDEFArea(Dictionary<string, Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>> entries, List<OxyColor> RBColors, int areaCount, Dictionary<string, bool> drawnArea, string areaName)
        {
            drawnArea[areaName] = true;
            var color = new SolidColorBrush(Color.FromArgb(RBColors[areaCount].A, RBColors[areaCount].R, RBColors[areaCount].G, RBColors[areaCount].B));
            areaCount++;
            var thisArea = entries[areaName];
            if (thisArea.Item1 == SignalMapPlotType.Dot)
            {
                var point = thisArea.Item2.FirstOrDefault();
                if (double.TryParse(point.Latitude, out double la) && double.TryParse(point.Longitude, out double lg))
                {
                    var mkr = new GMapMarker(new PointLatLng(la, lg));
                    mkr.Shape = new Ellipse
                    {
                        Width = 15,
                        Height = 15,
                        Stroke = color,
                        Fill = color,
                        ToolTip = areaName
                    };
                    mkr.Tag = areaName;
                    Gmap.Markers.Add(mkr);
                    mkr.ZIndex = 1000;
                    mkr.Offset = new Point(-8, -8);
                }
            }
            if (thisArea.Item1 == SignalMapPlotType.Line)
            {
                //collect all point pairs here if there's only 2 points in the line
                //if there's more than 2 points, draw straight line.
                List<CartesianPoint> points = new List<CartesianPoint>();
                var newLine = new List<PointLatLng>();
                for (int index = 0; index < thisArea.Item2.Count; index++)
                {
                    if (double.TryParse(thisArea.Item2[index].Longitude, out double lg) && double.TryParse(thisArea.Item2[index].Latitude, out double la))
                    {
                        newLine.Add(new PointLatLng(la, lg));
                    }
                }
                if (newLine.Count >= 2)
                {
                    //to plot them straight
                    var newRoute = new GMapRoute(newLine);
                    newRoute.Shape = new Path() { StrokeThickness = 4, ToolTip = areaName, Stroke = color};
                    newRoute.Tag = areaName;
                    Gmap.Markers.Add(newRoute);
                }
                else
                {
                    throw new Exception("A signal line should have at least 2 points/locations specified.");
                }
            }
            if (thisArea.Item1 == SignalMapPlotType.Area)
            {
                var points = new List<PointLatLng>();
                //var points2 = new List<Point>();
                foreach (var pnt in thisArea.Item2)
                {
                    if (double.TryParse(pnt.Latitude, out double la) && double.TryParse(pnt.Longitude, out double lg))
                    {
                        points.Add(new PointLatLng(la, lg));
                        //points2.Add(new Point(la, lg));
                    }
                }
                var mkr = new GMap.NET.WindowsPresentation.GMapPolygon(points);
                var areaColor = color.Clone();
                areaColor.Opacity = 0.2;
                mkr.Shape = new Path() { StrokeThickness = 2, ToolTip = areaName, Stroke = color, Fill = areaColor };
                mkr.Tag = areaName;
                Gmap.Markers.Add(mkr);
            }

            return areaCount;
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

        private void _setupMap()
        {
            Gmap = new GMapControl();
            Gmap.MaxZoom = MaxZoom;
            Gmap.MinZoom = MinZoom;
            Gmap.Zoom = 5;
            Gmap.CenterPosition = new PointLatLng(37.0902, -95.7129);
            Gmap.ShowCenter = false;
            Gmap.MapProvider = GMapProviders.OpenStreetMap;
            //Gmap.MapProvider = GoogleSatelliteMapProvider.Instance;
            Gmap.Manager.Mode = AccessMode.ServerAndCache;
            Gmap.CacheLocation = "..\\MapCache";
            Gmap.OnMapZoomChanged += _onMapZoomChanged;
            Gmap.IgnoreMarkerOnMouseWheel = true;
        }

        private void _onMapZoomChanged()
        {
            foreach (var marker in Gmap.Markers)
            {
                if (marker.Shape is Line && marker.Tag is Tuple<PointLatLng, PointLatLng>)
                {
                    var centers = marker.Tag as Tuple<PointLatLng, PointLatLng>;
                    var toAreaCenter = centers.Item1;
                    var fromAreaCenter = centers.Item2;
                    double upperx = 0d, lowerx = 0d, uppery = 0d, lowery = 0d, slope = 0d;
                    var tosp = Gmap.FromLatLngToLocal(toAreaCenter);
                    var frsp = Gmap.FromLatLngToLocal(fromAreaCenter);
                    var xDiff = tosp.X - frsp.X;
                    var yDiff = tosp.Y - frsp.Y;
                    if (xDiff != 0)
                    {
                        slope = -(double)yDiff / (double)xDiff; // as y axis in screen coordinate system point to downward, the slope has to be negated to match the cartesian coordiate system.
                    }
                    else
                    {
                        if (yDiff >= 0)
                        {
                            slope = double.PositiveInfinity;
                        }
                        else
                        {
                            slope = double.NegativeInfinity;
                        }
                    }
                    var upperAngle = Math.Atan(slope) + Math.PI / 5;
                    var lowerAngle = Math.Atan(slope) - Math.PI / 5;

                    var markerOffsetX = Math.Cos(Math.Atan(slope)) * 15;
                    var markerOffsetY = Math.Sin(Math.Atan(slope)) * 15;
                    double x2, y2;
                    if (fromAreaCenter.Lng >= toAreaCenter.Lng)
                    {
                        x2 = frsp.X - tosp.X - markerOffsetX;
                        y2 = frsp.Y - tosp.Y + markerOffsetY;
                    }
                    else
                    {
                        x2 = frsp.X - tosp.X + markerOffsetX;
                        y2 = frsp.Y - tosp.Y - markerOffsetY;
                    }
                    var thisline = marker.Shape as Line;
                    thisline.X2 = x2;
                    thisline.Y2 = y2;
                }
            }
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
