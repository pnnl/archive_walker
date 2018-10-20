using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Utilities;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace MapService.Models
{
    public class PointsPair
    {
        public PointsPair(CartesianPoint pointLatLng1, CartesianPoint pointLatLng2)
        {
            Point1 = pointLatLng1;
            Point2 = pointLatLng2;

            X1 = Point1.X;
            Y1 = Point1.Y;
            X2 = Point2.X;
            Y2 = Point2.Y;
            //Theta = Math.PI / 3.0;
            Phi = Math.PI / 2.0 - Theta / 2.0;
            Psi = Math.Atan2(Y2 - Y1, X2 - X1);
            DistanceBetweenPoints = Point1.Distance(Point2);
            Radius = DistanceBetweenPoints / (2.0 * Math.Sin(Theta / 2.0));
            Center1 = new CartesianPoint(X1 + Radius * Math.Sin(Math.PI / 2.0 - Phi + Psi), Y1 - Radius * Math.Cos(Math.PI / 2.0 - Phi + Psi));
            Center2 = new CartesianPoint(X2 - Radius * Math.Sin(Math.PI / 2.0 - Phi + Psi), Y2 + Radius * Math.Cos(Math.PI / 2.0 - Phi + Psi));
            AngleCenter1Point1 = Math.Atan2(Y1 - Center1.Y, X1 - Center1.X);
            AngleCenter1Point2 = Math.Atan2(Y2 - Center1.Y, X2 - Center1.X);
            var Ts1 = Utility.UnWrap(new List<double> { AngleCenter1Point1, AngleCenter1Point2 });
            var T1 = Vector<double>.Build.DenseOfArray(Generate.LinearRange(Ts1[0], (Ts1[1] - Ts1[0])/NumberOfPointsForEachCurve, Ts1[1]));
            var xx1 = T1.PointwiseCos().Multiply(Radius).Add(Center1.X);
            xx1[0] = X1;
            xx1[xx1.Count - 1] = X2;
            var yy1 = T1.PointwiseSin().Multiply(Radius).Add(Center1.Y);
            yy1[0] = Y1;
            yy1[yy1.Count - 1] = Y2;
            Curve1 = new List<CartesianPoint>();
            for (int index = 0; index < xx1.Count; index++)
            {
                Curve1.Add(new CartesianPoint(xx1[index], yy1[index]));
            }
            var AngleCenter2Point1 = Math.Atan2(Y1 - Center2.Y, X1 - Center2.X);
            var AngleCenter2Point2 = Math.Atan2(Y2 - Center2.Y, X2 - Center2.X);
            var Ts2 = Utility.UnWrap(new List<double> { AngleCenter2Point1, AngleCenter2Point2 });
            var T2 = Vector<double>.Build.DenseOfArray(Generate.LinearRange(Ts2[0], (Ts2[1] - Ts2[0])/NumberOfPointsForEachCurve, Ts2[1]));
            var xx2 = T2.PointwiseCos().Multiply(Radius).Add(Center2.X);
            xx2[0] = X1;
            xx2[xx2.Count - 1] = X2;
            var yy2 = T2.PointwiseSin().Multiply(Radius).Add(Center2.Y);
            yy2[0] = Y1;
            yy2[yy2.Count - 1] = Y2;
            Curve2 = new List<CartesianPoint>();
            for (int index = 0; index < xx2.Count; index++)
            {
                Curve2.Add(new CartesianPoint(xx2[index], yy2[index]));
            }
            SelectedCurve = new List<CartesianPoint>();
        }
        //private PointLatLng _point1;
        public CartesianPoint Point1 { get; set; }
        //private PointLatLng _point2;
        public CartesianPoint Point2 { get; set; }
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public double Theta { get; set; } = Math.PI / 3.0;
        public double Phi { get; set; }
        public double Psi { get; set; }
        public double DistanceBetweenPoints { get; set; }
        public double Radius { get; set; }
        public CartesianPoint Center1 { get; set; }
        public CartesianPoint Center2 { get; set; }
        public double AngleCenter1Point1 { get; set; }
        public double AngleCenter1Point2 { get; set; }
        public double AngleCenter2Point1 { get; set; }
        public double AngleCenter2Point2 { get; set; }
        public int NumberOfPointsForEachCurve { get; set; } = 100;
        public List<CartesianPoint> Curve1 { get; set; }
        public List<CartesianPoint> Curve2 { get; set; }
        public List<CartesianPoint> SelectedCurve { get; set; }
        public CartesianPoint SelectedCenter { get; set; }
    }
}
