using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Utilities;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using MapService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapService.ViewModels
{
    public class ResultMapViewModel : ViewModelBase
    {
        public ResultMapViewModel()
        {
            _curveDesignOption = CurveDesignOptions.Option1;
            _setupMap();
        }
        private List<SignalSignatureViewModel> _signals;
        public List<SignalSignatureViewModel> Signals
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
            var linePairs = new List<PointsPair>();
            foreach (var signal in Signals)
            {
                if (signal.MapPlotType == SignalMapPlotType.Dot)
                {
                    _addDotToMap();
                }
                if (signal.MapPlotType == SignalMapPlotType.Line)
                {
                    //collect all point pairs here a list of tuples, maybe?
                    List<CartesianPoint> points = new List<CartesianPoint>();
                    for (int index = 0; index < signal.Locations.Count; index++)
                    {
                        points.Add(new CartesianPoint(double.Parse(signal.Locations[index].Longitude), double.Parse(signal.Locations[index].Latitude)));
                    }
                    for (int index = 0; index < points.Count - 1; index++)
                    {
                        linePairs.Add(new PointsPair(points[index], points[index + 1]));
                    }
                }
                if (signal.MapPlotType == SignalMapPlotType.Area)
                {
                    _addPolygonToMap();
                }
            }
            if (linePairs.Count != 0)
            {
                _drawCurvesOnMap(linePairs);
            }
        }

        private void _drawCurvesOnMap(List<PointsPair> linePairs)
        {
            //int numberOfPointsForEachCurve = 100;
            List< GMapRoute> curveList = _designCurves(linePairs);
            foreach (var curve in curveList)
            {
                //TODO: change some properties of each curve so they look better
                Gmap.Markers.Add(curve);
            }
        }

        private List<GMapRoute> _designCurves(List<PointsPair> linePairs)
        {
            List<GMapRoute> curveList = new List<GMapRoute>();
            var cuvreDistanceMatrix = new List<CurveDistanceHolder>();
            for (int index = 0; index < linePairs.Count - 1; index++)
            {
                for (int index2 = index + 1; index2 < linePairs.Count; index2++)
                {
                    cuvreDistanceMatrix.Add(new CurveDistanceHolder(linePairs[index], linePairs[index2]));
                    //double distance1 = _distanceBetween2Curves(pointPair1.Curve1, pointPair2.Curve1);
                    //double distance2 = _distanceBetween2Curves(pointPair1.Curve1, pointPair2.Curve2);
                    //double distance3 = _distanceBetween2Curves(pointPair1.Curve2, pointPair2.Curve1);
                    //double distance4 = _distanceBetween2Curves(pointPair1.Curve2, pointPair2.Curve2);
                }
            }
            var curveDistanceMatrixCopy = cuvreDistanceMatrix;
            CurveDistanceHolder selectedpair = null;
            while (cuvreDistanceMatrix.Count != 0)
            {
                if (CurveDesignOption == CurveDesignOptions.Option1)
                {
                    //cureDistanceMatrix.Select(x => x.MinDistance).Min();
                    selectedpair = cuvreDistanceMatrix.Aggregate((curMin, x) => x.MinDistance < curMin.MinDistance ? x : curMin);
                }
                else
                {
                    selectedpair = cuvreDistanceMatrix.Aggregate((curMax, x) => x.MaxDistanceDiff > curMax.MaxDistanceDiff ? x : curMax);
                }
                if (selectedpair != null)
                {
                    selectedpair.SetCurveSelection();
                    cuvreDistanceMatrix = cuvreDistanceMatrix.Where(x => x.PointPair1 != selectedpair.PointPair1 && x.PointPair2 != selectedpair.PointPair2).ToList();
                    //cureDistanceMatrix.RemoveAll(x => x.PointPair1 == selectedpair.PointPair1 || x.PointPair2 == selectedpair.PointPair2);
                    //curveList.Add(new GMapRoute(selectedpair.PointPair1.SelectedCurve);
                    //curveList.Add(selectedpair.PointPair2.SelectedCurve);
                }
                foreach (var pair in linePairs)
                {
                    if (pair.SelectedCurve.Count == 0)
                    {
                        if ((pair.X1 == selectedpair.PointPair1.X1 && pair.Y1 == selectedpair.PointPair1.Y1 && pair.X2 == selectedpair.PointPair1.X2 && pair.Y2 == selectedpair.PointPair1.Y2)
                        || (pair.X1 == selectedpair.PointPair1.X2 && pair.Y1 == selectedpair.PointPair1.Y2 && pair.X2 == selectedpair.PointPair1.X1 && pair.Y2 == selectedpair.PointPair1.Y1))
                        {
                            if(pair.Center1.Distance(selectedpair.PointPair1.SelectedCenter) < pair.Center2.Distance(selectedpair.PointPair1.SelectedCenter))
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
                        else if ((pair.X1 == selectedpair.PointPair2.X1 && pair.Y1 == selectedpair.PointPair2.Y1 && pair.X2 == selectedpair.PointPair2.X2 && pair.Y2 == selectedpair.PointPair2.Y2)
                        || (pair.X1 == selectedpair.PointPair2.X2 && pair.Y1 == selectedpair.PointPair2.Y2 && pair.X2 == selectedpair.PointPair2.X1 && pair.Y2 == selectedpair.PointPair2.Y1))
                        {
                            if (pair.Center1.Distance(selectedpair.PointPair2.SelectedCenter) < pair.Center2.Distance(selectedpair.PointPair2.SelectedCenter))
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
            var remainedPair = linePairs.Find(x => x.SelectedCurve.Count == 0);
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
            foreach (var pair in linePairs)
            {
                var newCurve = new List<PointLatLng>();
                foreach (var p in pair.SelectedCurve)
                {
                    newCurve.Add(new PointLatLng(p.Y, p.X));
                }
                curveList.Add(new GMapRoute(newCurve));
            }
            return curveList;
        }

        public void ClearMarkers()
        {
            throw new NotImplementedException();
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

        public void UpdateResultMap(List<SignalSignatureViewModel> signalList)
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
    }
}
