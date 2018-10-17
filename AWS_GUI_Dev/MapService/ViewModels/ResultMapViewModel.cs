using BAWGUI.Core;
using BAWGUI.Core.Models;
using BAWGUI.Utilities;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapService.ViewModels
{
    public class ResultMapViewModel : ViewModelBase
    {
        public ResultMapViewModel()
        {
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
            var linePairs = new List<Tuple<PointLatLng, PointLatLng>>();
            foreach (var signal in Signals)
            {
                if (signal.MapPlotType == SignalMapPlotType.Dot)
                {
                    _addDotToMap();
                }
                if (signal.MapPlotType == SignalMapPlotType.Line)
                {
                    //collect all point pairs here a list of tuples, maybe?
                    List<PointLatLng> points = new List<PointLatLng>();
                    for (int index = 0; index < signal.Locations.Count - 1; index++)
                    {
                        points.Add(new PointLatLng(double.Parse(signal.Locations[index].Latitude), double.Parse(signal.Locations[index].Longitude)));
                    }
                    for (int index = 0; index < points.Count - 2; index++)
                    {
                        linePairs.Add(new Tuple<PointLatLng, PointLatLng>(points[index], points[index + 1]));
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

        private void _drawCurvesOnMap(List<Tuple<PointLatLng, PointLatLng>> linePairs)
        {
            int numberOfPointsForEachCurve = 100;
            List< GMapRoute> curveList = _designCurves(linePairs, numberOfPointsForEachCurve);
            foreach (var curve in curveList)
            {
                //TODO: change some properties of each curve so they look better
                Gmap.Markers.Add(curve);
            }
        }

        private List<GMapRoute> _designCurves(List<Tuple<PointLatLng, PointLatLng>> linePairs, int numberOfPointsForEachCurve)
        {
            List<GMapRoute> curveList = new List<GMapRoute>();
            foreach (var pair in linePairs)
            {
                var X1 = pair.Item1.Lat;
                var Y1 = pair.Item1.Lng;
                var X2 = pair.Item2.Lat;
                var Y2 = pair.Item2.Lng;
                var theta = Math.PI / 3.0;
                var phi = Math.PI / 2.0 - theta / 2.0;
                var psi = Math.Atan2(Y2 - Y1, X2 - X1);
                var d = Math.Sqrt(Math.Pow((X2 - X1), 2) + Math.Pow((Y2 - Y1), 2));
                var r = d / (2.0 * Math.Sin(theta / 2.0));
                var c1 = new Tuple<double, double>(X1 + r * Math.Sin(Math.PI / 2.0 - phi + psi), Y1 - r * Math.Cos(Math.PI / 2.0 - phi + psi));
                var c2 = new Tuple<double, double>(X2 - r * Math.Sin(Math.PI / 2.0 - phi + psi), Y2 + r * Math.Cos(Math.PI / 2.0 - phi + psi));
                var T1 = Math.Atan2(Y1 - c1.Item2, X1 - c1.Item1);
                var T2 = Math.Atan2(Y2 - c1.Item2, X2 - c1.Item1);
                var Ts = Utility.UnWrap(new List<double> { T1, T2 });
                var T = Vector<double>.Build.DenseOfArray(Generate.LinearRange(Ts[0], numberOfPointsForEachCurve, Ts[1]));
            }
            return curveList;
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
            throw new NotImplementedException();
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
