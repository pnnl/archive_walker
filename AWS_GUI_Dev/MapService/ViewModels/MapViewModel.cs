using BAWGUI.Utilities;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using MapService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.IO;

namespace MapService.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        public MapViewModel()
        {
            SetUpGMap();
            Annotations = new List<PointAndInfo>();
            ChangeZoom = new RelayCommand(_changeMapZoom);
            ZoomIn = new RelayCommand(_mapZoomIn);
            ZoomOut = new RelayCommand(_mapZoomOut);
            IsOfflineMode = true;
            //MaxZoom = 7;
            //MinZoom = 0;
            CacheSelectedMapRegion = new RelayCommand(_cacheSelectedMapRegion);
            //GMap.InvalidateVisual();
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
        public int MaxZoom { get; set; } = 8;
        public int MinZoom { get; set; } = 0;
        public void SetUpGMap()
        {
            try
            {
                Gmap = new GMapControl();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            //GMap.MapProvider = OpenStreetMapProvider.Instance;
            Gmap.MaxZoom = MaxZoom;
            Gmap.MinZoom = MinZoom;
            Gmap.Zoom = 5;
            Gmap.CenterPosition = new PointLatLng(37.0902, -95.7129);
            Gmap.ShowCenter = false;

            Gmap.MapProvider = GMapProviders.OpenStreetMap;
            if (!IsOfflineMode)
            {
                Gmap.Manager.Mode = AccessMode.ServerAndCache;
            }
#if DEBUG
            Gmap.CacheLocation = "..\\MapCache";
#else
            Gmap.CacheLocation = ".\\MapCache";
#endif
            Gmap.MouseMove += GMap_MouseMove;
            Gmap.MouseLeftButtonDown += GMap_MouseLeftButtonDown;

            //GMap.Position = new PointLatLng(54.6961334816182, 25.2985095977783);

            //MainMap.ScaleMode = ScaleModes.Dynamic;

            // map events
            //GMap.OnPositionChanged += new PositionChanged(MainMap_OnCurrentPositionChanged);
            //GMap.OnTileLoadComplete += new TileLoadComplete(MainMap_OnTileLoadComplete);
            //GMap.OnTileLoadStart += new TileLoadStart(MainMap_OnTileLoadStart);
            //GMap.OnMapTypeChanged += new MapTypeChanged(MainMap_OnMapTypeChanged);
            //GMap.MouseMove += new System.Windows.Input.MouseEventHandler(MainMap_MouseMove);
            //GMap.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(MainMap_MouseLeftButtonDown);
            //GMap.MouseEnter += new MouseEventHandler(MainMap_MouseEnter);
        }
        static readonly string ReverseGeocoderUrlFormat = "http://nominatim.openstreetmap.org/reverse?format=xml&lat={0}&lon={1}&zoom=18&addressdetails=1";
        private void GMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var gctl = sender as GMapControl;
            var ps = e.GetPosition(gctl);
            var latlng = gctl.FromLocalToLatLng((int)ps.X, (int)ps.Y);
            var locationStr = string.Format("Latitude: {0}, longitude: {1}\n", latlng.Lat, latlng.Lng);
            //var info = new GeoCoderStatusCode();
            if (Gmap.Manager.Mode != AccessMode.CacheOnly)
            {
                var url = string.Format(CultureInfo.InvariantCulture, ReverseGeocoderUrlFormat, latlng.Lat, latlng.Lng);
                //var st = GMapProviders.OpenStreetMap.GetPlacemark(latlng, out info);
                XmlDocument xmldoc = new XmlDocument();
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                Stream data = client.OpenRead(url);
                StreamReader reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                xmldoc.LoadXml(s);
                XmlNode r = xmldoc.SelectSingleNode("/reversegeocode/result");
                if (r != null)
                {
                    var p = new Placemark();
                    XmlNode ad = xmldoc.SelectSingleNode("/reversegeocode/addressparts");
                    if (ad != null)
                    {
                        var vl = ad.SelectSingleNode("country");
                        if (vl != null)
                        {
                            p.CountryName = vl.InnerText;
                        }

                        vl = ad.SelectSingleNode("country_code");
                        if (vl != null)
                        {
                            p.CountryNameCode = vl.InnerText;
                        }

                        vl = ad.SelectSingleNode("postcode");
                        if (vl != null)
                        {
                            p.PostalCodeNumber = vl.InnerText;
                        }

                        vl = ad.SelectSingleNode("state");
                        if (vl != null)
                        {
                            p.AdministrativeAreaName = vl.InnerText;
                        }

                        vl = ad.SelectSingleNode("region");
                        if (vl != null)
                        {
                            p.SubAdministrativeAreaName = vl.InnerText;
                        }

                        vl = ad.SelectSingleNode("suburb");
                        if (vl != null)
                        {
                            p.LocalityName = vl.InnerText;
                        }

                        vl = ad.SelectSingleNode("road");
                        if (vl != null)
                        {
                            p.ThoroughfareName = vl.InnerText;
                        }
                    }
                    data.Close();
                    reader.Close();
                    var streetStr = string.Format("{0}, {1}\n{2}, {3} {4}\n{5}, {6}", p.ThoroughfareName, p.LocalityName, p.SubAdministrativeAreaName, p.AdministrativeAreaName, p.PostalCodeNumber, p.CountryName, p.CountryNameCode);
                    locationStr = locationStr + "\n" + streetStr;
                }
                //var st = GetPlacemarkFromReverseGeocoderUrl(url, out info); ;
            }

            var result = System.Windows.Forms.MessageBox.Show("Add location:\n" + locationStr + "\nto coordinate table?", "Warning!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                OnLocationSelected(latlng);
            }
        }
        public event EventHandler<PointLatLng> LocationSelected;
        protected virtual void OnLocationSelected(PointLatLng e)
        {
            LocationSelected?.Invoke(this, e);
        }
        private void GMap_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var gctl = sender as GMapControl;
            //if (gctl == GMap)
            //{
            //    CurrentLng = gctl.Position.Lng;
            //    CurrentLat = gctl.Position.Lat;
            //}
            var ps = e.GetPosition(gctl);
            CurrentLng = gctl.FromLocalToLatLng((int)ps.X, (int)ps.Y).Lng;
            CurrentLat = gctl.FromLocalToLatLng((int)ps.X, (int)ps.Y).Lat;
        }
        private double _currentLat;
        public double CurrentLat
        {
            get { return _currentLat; }
            set
            {
                _currentLat = value;
                OnPropertyChanged();
            }
        }
        private double _currentLng;
        public double CurrentLng
        {
            get { return _currentLng; }
            set
            {
                _currentLng = value;
                OnPropertyChanged();
            }
        }
        public List<PointAndInfo> Annotations { get; set; }
        //private void MainMap_MouseMove(object sender, MouseEventArgs e)
        //{
        //}

        //private void MainMap_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    GMap.Focus();
        //}

        //private void MainMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //}

        //private void MainMap_OnMapTypeChanged(GMapProvider type)
        //{
        //}

        //private void MainMap_OnTileLoadStart()
        //{
        //}

        //private void MainMap_OnTileLoadComplete(long ElapsedMilliseconds)
        //{
        //}

        //private void MainMap_OnCurrentPositionChanged(PointLatLng point)
        //{
        //    //mapgroup.Header = "gmap: " + point;
        //}

        public void ModifyMapAnnotation()
        {
            //if (isChecked)
            //{
            //    Annotations.Add(new PointAndInfo(location, id, siteName));
            //}
            //else
            //{
            //    DeleteAnnotation(id);
            //}
            Gmap.Markers.Clear();
            foreach (var item in Annotations)
            {
                var newMarker = new GMapMarker(item.Point);
                {
                    //newMarker.Shape = new CustomMarkerRed(newMarker, item.Info);
                    var local = Gmap.FromLatLngToLocal(item.Point);
                    //newMarker.Map = GMap;
                    newMarker.Offset = new System.Windows.Point(-12.5, -25);
                    //newMarker.ZIndex = int.MaxValue;
                    //newMarker.LocalPositionX = local.X;
                    //newMarker.LocalPositionY = local.Y;
                    newMarker.Shape = new Image
                    {
                        Width = 25,
                        Height = 25,
                        Source = new BitmapImage(new System.Uri(@"..\..\..\..\MyResources\bigMarkerGreen.png", UriKind.Relative))
                    };
                }
                Gmap.Markers.Add(newMarker);
            }
        }

        public ICommand ChangeZoom { get; set; }
        private void _changeMapZoom(object obj)
        {
            Gmap.Zoom = Gmap.Zoom;
        }
        public ICommand ZoomIn { get; set; }
        private void _mapZoomIn(object obj)
        {
            Gmap.Zoom = Gmap.Zoom + 1;
        }
        public ICommand ZoomOut { get; set; }
        private void _mapZoomOut(object obj)
        {
            Gmap.Zoom = Gmap.Zoom - 1;
        }
        private bool _isOfflineMode;
        public bool IsOfflineMode
        {
            get { return _isOfflineMode; }
            set
            {
                _isOfflineMode = value;
                if (value)
                {
                    Gmap.Manager.Mode = AccessMode.CacheOnly;
                }
                else
                {
                    Gmap.Manager.Mode = AccessMode.ServerAndCache;
                }
                OnPropertyChanged();
            }
        }
        public ICommand CacheSelectedMapRegion { get; set; }
        private void _cacheSelectedMapRegion(object obj)
        {
            RectLatLng area = Gmap.SelectedArea;
            if (!area.IsEmpty)
            {
                for (int i = (int)Gmap.Zoom; i <= Gmap.MaxZoom; i++)
                {
                    var res = System.Windows.Forms.MessageBox.Show("Ready ripp at Zoom = " + i + " ?", "GMap.NET", MessageBoxButtons.YesNoCancel);

                    if (res == DialogResult.Yes)
                    {
                        GMap.NET.WindowsPresentation.TilePrefetcher tileFetcher = new GMap.NET.WindowsPresentation.TilePrefetcher();
                        //tileFetcher.Owner = this;
                        tileFetcher.ShowCompleteMessage = true;
                        tileFetcher.Start(area, i, Gmap.MapProvider, 100);
                    }
                    else if (res == DialogResult.No)
                    {
                        continue;
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        break;
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Select map area holding ALT", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        public void DeleteAnnotation(int internalID)
        {
            foreach (var item in Annotations)
            {
                if (item.ID == internalID)
                {
                    Annotations.Remove(item);
                    break;
                }
            }
        }
        public void AddAnnotation(PointLatLng location, int id, string siteName)
        {
            var exist = Annotations.Any(x => x.ID == id);
            if (!exist)
            {
                Annotations.Add(new PointAndInfo(location, id, siteName));
            }
        }
    }
}
