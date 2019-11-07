using BAWGUI.Core;
using BAWGUI.Core.Models;
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
            var RBColors = OxyPalettes.Jet(entries.Count()).Colors.ToList();
            int areaCount = 0;
            Gmap.Markers.Clear();
            var drawnArea = new List<string>();
            //var sortedPath = paths.OrderByDescending(x => Math.Abs(x.DEF)).ToList();
            //var thinkest = Math.Abs(sortedPath.FirstOrDefault().DEF);
            var sortedPath = paths.OrderBy(x => Math.Abs(x.DEF)).ToList();
            var thinnest = Math.Abs(sortedPath.FirstOrDefault().DEF);//actual thinnest
            foreach (var pth in paths)
            {
                if (!drawnArea.Contains(pth.From))
                {
                    areaCount = _drawDEFArea(entries, RBColors, areaCount, drawnArea, pth.From);
                }
                if (!drawnArea.Contains(pth.To))
                {
                    areaCount = _drawDEFArea(entries, RBColors, areaCount, drawnArea, pth.To);
                }
                _drawArrow(entries, pth, thinnest);
            }
            Gmap.InvalidateVisual(false);
            Gmap.UpdateLayout();
        }

        private void _drawArrow(Dictionary<string, Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>> entries, ForcedOscillationTypeOccurrencePath pth, float scaleAgainst)
        {
            PointLatLng fromAreaCenter = new PointLatLng();
            PointLatLng toAreaCenter = new PointLatLng();
            SignalMapPlotType toCenterType;
            string pathLabel = "";
            string arrowHeadLabel = "";
            if (pth.DEF > 0)
            {
                fromAreaCenter = _getAreaCenter(entries[pth.From].Item2);
                toAreaCenter = _getAreaCenter(entries[pth.To].Item2);
                toCenterType = entries[pth.To].Item1;
                pathLabel = pth.From + " to " + pth.To;
                arrowHeadLabel = pth.To + "_arrow_head";
            }
            else
            {
                fromAreaCenter = _getAreaCenter(entries[pth.To].Item2);
                toAreaCenter = _getAreaCenter(entries[pth.From].Item2);
                toCenterType = entries[pth.From].Item1;
                pathLabel = pth.To + " to " + pth.From;
                arrowHeadLabel = pth.From + "_arrow_head";
            }
            //var relativeEFstrength = (int)(Math.Abs(Math.Log10(Math.Abs(pth.DEF)) / Math.Log10(thinkest)) * 10);
            var relativeEFstrength = (int)(Math.Abs(Math.Log10(Math.Abs(pth.DEF) / scaleAgainst)) * 2) + 4;
            var arrowHeadSize = relativeEFstrength * 1.5;
            double upperx = 0d, lowerx = 0d, uppery = 0d, lowery = 0d, slope = 0d;
            var xDiff = fromAreaCenter.Lng - toAreaCenter.Lng;
            var yDiff = fromAreaCenter.Lat - toAreaCenter.Lat;
            if (xDiff != 0)
            {
                slope = yDiff / xDiff;
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
            Console.WriteLine("angles: arrow body: {2} upper: {0}, lower: {1}", upperAngle * 180 / Math.PI, lowerAngle * 180 / Math.PI, Math.Atan(slope) *180 /Math.PI);

            var sp = Gmap.FromLatLngToLocal(toAreaCenter);
            Console.WriteLine("screen position: x: {0}, y: {1}", sp.X, sp.Y);
            if (fromAreaCenter.Lng >= toAreaCenter.Lng)
            {
                upperx = sp.X + Math.Cos(upperAngle) * arrowHeadSize;
                uppery = sp.Y - Math.Sin(upperAngle) * arrowHeadSize;
                lowerx = sp.X + Math.Cos(lowerAngle) * arrowHeadSize;
                lowery = sp.Y - Math.Sin(lowerAngle) * arrowHeadSize;
            }
            else
            {
                upperx = sp.X - Math.Cos(upperAngle) * arrowHeadSize;
                uppery = sp.Y + Math.Sin(upperAngle) * arrowHeadSize;
                lowerx = sp.X - Math.Cos(lowerAngle) * arrowHeadSize;
                lowery = sp.Y + Math.Sin(lowerAngle) * arrowHeadSize;
            }
            Console.WriteLine("upper screen position: x: {0}, y: {1}", upperx, uppery);
            Console.WriteLine("lower screen position: x: {0}, y: {1}", lowerx, lowery);

            var pointsCollection = new PointCollection();
            //pointsCollection.Add(new Point(sp.X - sp.X, sp.Y - sp.Y));
            //pointsCollection.Add(new Point(upperx - sp.X, uppery - sp.Y));
            //pointsCollection.Add(new Point(lowerx - sp.X, lowery - sp.Y));
            var newOffsetx = (upperx + lowerx) / 2;
            var newOffsety = (uppery + lowery) / 2;
            pointsCollection.Add(new Point(sp.X - newOffsetx, sp.Y - newOffsety));
            pointsCollection.Add(new Point(upperx - newOffsetx, uppery - newOffsety));
            pointsCollection.Add(new Point(lowerx - newOffsetx, lowery - newOffsety));

            var markerOffsetX = Math.Cos(Math.Atan(slope)) * 15;
            var markerOffsetY = Math.Sin(Math.Atan(slope)) * 15;
            Point markerOffset;
            if (toCenterType == SignalMapPlotType.Dot)
            {
                if (fromAreaCenter.Lng >= toAreaCenter.Lng)
                {
                    //from location is in the first or 4th quadrant of to location
                    markerOffset = new Point(markerOffsetX, -markerOffsetY);
                }
                else
                {
                    //from location is in the 2nd or 3rd quadrant of to location
                    markerOffset = new Point(-markerOffsetX, markerOffsetY);
                }
            }
            else
            {
                markerOffset = new Point(0, 0);
            }

            var mkr = new GMapMarker(toAreaCenter);
            mkr.Shape = new Polygon
            {
                Stroke = Brushes.Black,
                Fill = Brushes.Black,
                Points = pointsCollection,
                ToolTip = arrowHeadLabel
            };
            mkr.Tag = arrowHeadLabel;
            mkr.Offset = markerOffset;
            Gmap.Markers.Add(mkr);

            var upperlatlng = Gmap.FromLocalToLatLng((int)upperx, (int)uppery);
            var lowerlatlng = Gmap.FromLocalToLatLng((int)lowerx, (int)lowery);
            var newline = new List<PointLatLng>();
            newline.Add(fromAreaCenter);
            newline.Add(toAreaCenter);
            //newline.Add(_getTriangleCenter(toAreaCenter, upperlatlng, lowerlatlng));

            var newRoute = new GMapRoute(newline);
            newRoute.Shape = new Path() { StrokeThickness = relativeEFstrength, ToolTip = pathLabel, Stroke = Brushes.Black, StrokeEndLineCap = PenLineCap.Triangle };
            newRoute.Tag = pathLabel;
            newRoute.Offset = markerOffset;
            Gmap.Markers.Add(newRoute);
            Gmap.RegenerateShape(newRoute);
            
            //var newRoute2 = new GMapRoute(newline);
            //newRoute2.Shape = new Path() { StrokeThickness = 4, ToolTip = label, Stroke = Brushes.Black, StrokeEndLineCap = PenLineCap.Triangle };
            //newRoute2.Tag = label;
            //Gmap.Markers.Add(newRoute2);

        }

        private PointLatLng _getTriangleCenter(PointLatLng a, PointLatLng b, PointLatLng c)
        {
            return new PointLatLng((a.Lat + b.Lat + c.Lat) / 3, (a.Lng +b.Lng + c.Lng)/3);
        }

        private PointLatLng _getAreaCenter(List<SiteCoordinatesModel> locations)
        {
            var lng = locations.Average(x => double.Parse(x.Longitude));
            var lat = locations.Average(x => double.Parse(x.Latitude));
            return new PointLatLng(lat, lng);
        }

        private int _drawDEFArea(Dictionary<string, Tuple<SignalMapPlotType, List<SiteCoordinatesModel>>> entries, List<OxyColor> RBColors, int areaCount, List<string> drawnArea, string areaName)
        {
            drawnArea.Add(areaName);
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
                    mkr.Offset = new Point(-8, -8);
                    Gmap.Markers.Add(mkr);
                    mkr.ZIndex = 1000;
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
                var points2 = new List<Point>();
                foreach (var pnt in thisArea.Item2)
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
            //Gmap.MapProvider = GoogleSatelliteMapProvider.Instance;
            Gmap.Manager.Mode = AccessMode.ServerAndCache;
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
