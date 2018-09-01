using BAWGUI.Utilities;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using MapService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            IsOnlineMode = true;
            CacheSelectedMapRegion = new RelayCommand(_cacheSelectedMapRegion);
            //GMap.InvalidateVisual();
        }
        private GMapControl _gMap;
        public GMapControl GMap
        {
            get { return _gMap; }
            set
            {
                _gMap = value;
                OnPropertyChanged();
            }
        }
        public void SetUpGMap()
        {
            GMap = new GMapControl();
            //GMap.MapProvider = OpenStreetMapProvider.Instance;
            GMap.MaxZoom = 10;
            GMap.MinZoom = 0;
            GMap.Zoom = 5;
            GMap.CenterPosition = new PointLatLng(37.0902, -95.7129);
            GMap.ShowCenter = false;

            GMap.MapProvider = GMapProviders.OpenStreetMap;
            if (!IsOnlineMode)
            {
                GMap.Manager.Mode = AccessMode.CacheOnly;
            }
            GMap.CacheLocation = "..\\MapCache";
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

        public void ModifyMapAnnotation(PointLatLng location, string siteName, bool isChecked)
        {
            if (isChecked)
            {
                Annotations.Add(new PointAndInfo(location, siteName));
            }
            else
            {
                foreach (var item in Annotations)
                {
                    if (item.Info == siteName)
                    {
                        Annotations.Remove(item);
                        break;
                    }
                }
            }
            GMap.Markers.Clear();
            foreach (var item in Annotations)
            {
                var newMarker = new GMapMarker(item.Point);
                {
                    //newMarker.Shape = new CustomMarkerRed(newMarker, item.Info);
                    var local = GMap.FromLatLngToLocal(item.Point);
                    //newMarker.Map = GMap;
                    newMarker.Offset = new System.Windows.Point(-12.5, -25);
                    //newMarker.ZIndex = int.MaxValue;
                    //newMarker.LocalPositionX = local.X;
                    //newMarker.LocalPositionY = local.Y;
                    newMarker.Shape = new Image
                    {
                        Width = 25,
                        Height = 25,
                        Source = new BitmapImage(new System.Uri(@"C:\Users\wang690\Desktop\projects\TIP348\VoltageStability\archive_walker\AWS_GUI_Dev\BAWGUI.Resources\Images\bigMarkerGreen.png"))
                    };
                }
                GMap.Markers.Add(newMarker);
            }
        }

        public ICommand ChangeZoom { get; set; }
        private void _changeMapZoom(object obj)
        {
            GMap.Zoom = GMap.Zoom;
        }
        public ICommand ZoomIn { get; set; }
        private void _mapZoomIn(object obj)
        {
            GMap.Zoom = GMap.Zoom + 1;
        }
        public ICommand ZoomOut { get; set; }
        private void _mapZoomOut(object obj)
        {
            GMap.Zoom = GMap.Zoom - 1;
        }
        private bool _isOnlineMode;
        public bool IsOnlineMode
        {
            get { return _isOnlineMode; }
            set
            {
                _isOnlineMode = value;
                if (value)
                {
                    GMap.Manager.Mode = AccessMode.ServerAndCache;
                }
                else
                {
                    GMap.Manager.Mode = AccessMode.CacheOnly;
                }
                OnPropertyChanged();
            }
        }
        public ICommand CacheSelectedMapRegion { get; set; }
        private void _cacheSelectedMapRegion(object obj)
        {
            RectLatLng area = GMap.SelectedArea;
            if (!area.IsEmpty)
            {
                for (int i = (int)GMap.Zoom; i <= GMap.MaxZoom; i++)
                {
                    MessageBoxResult res = MessageBox.Show("Ready ripp at Zoom = " + i + " ?", "GMap.NET", MessageBoxButton.YesNoCancel);

                    if (res == MessageBoxResult.Yes)
                    {
                        TilePrefetcher tileFetcher = new TilePrefetcher();
                        //tileFetcher.Owner = this;
                        tileFetcher.ShowCompleteMessage = true;
                        tileFetcher.Start(area, i, GMap.MapProvider, 100);
                    }
                    else if (res == MessageBoxResult.No)
                    {
                        continue;
                    }
                    else if (res == MessageBoxResult.Cancel)
                    {
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Select map area holding ALT", "GMap.NET", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
    }
}
