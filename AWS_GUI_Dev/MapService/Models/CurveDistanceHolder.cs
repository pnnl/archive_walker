using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapService.Models
{
    public class CurveDistanceHolder
    {
        public CurveDistanceHolder(PointsPair p1, PointsPair p2)
        {
            PointPair1 = p1;
            PointPair2 = p2;
            Distance11 = _distanceBetween2Curves(p1.Curve1, p2.Curve1);
            Distance12 = _distanceBetween2Curves(p1.Curve1, p2.Curve2);
            Distance21 = _distanceBetween2Curves(p1.Curve2, p2.Curve1);
            Distance22 = _distanceBetween2Curves(p1.Curve2, p2.Curve2);
            _findMinMaxDistance();
        }
        public PointsPair PointPair1 { get; set; }
        public PointsPair PointPair2 { get; set; }
        public List<double> Distances { get; set; }
        public double Distance11 { get; set; }
        public double Distance12 { get; set; }
        public double Distance21 { get; set; }
        public double Distance22 { get; set; }
        public double MinDistance { get; set; }
        public double MaxDistance { get; set; }
        public double MaxIdx { get; set; }
        public double MaxDistanceDiff { get; set; }

        private void _findMinMaxDistance()
        {
            var min1 = 0d;
            var max1 = 0d;
            var min2 = 0d;
            var max2 = 0d;
            var maxIdx1 = 0;
            var maxIdx2 = 0;
            if (Distance11 <= Distance12)
            {
                min1 = Distance11;
                max1 = Distance12;
                maxIdx1 = 1;
            }
            else
            {
                min1 = Distance12;
                max1 = Distance11;
                maxIdx1 = 0;
            }
            if (Distance21 <= Distance22)
            {
                min2 = Distance21;
                max2 = Distance22;
                maxIdx2 = 3;
            }
            else
            {
                min2 = Distance22;
                max2 = Distance21;
                maxIdx2 = 2;
            }
            if (min1 <= min2)
            {
                MinDistance = min1;
            }
            else
            {
                MinDistance = min2;
            }
            if (max1 >= max2)
            {
                MaxDistance = max1;
                MaxIdx = maxIdx1;
            }
            else
            {
                MaxDistance = max2;
                MaxIdx = maxIdx2;
            }
            MaxDistanceDiff = MaxDistance - MinDistance;
        }

        private double _distanceBetween2Curves(List<CartesianPoint> curve1, List<CartesianPoint> curve2)
        {
            var xAccDiff = 0d;
            var yAccDiff = 0d;
            var xAccDiffRev = 0d;
            var yAccDiffRev = 0d;
            for (int index = 0; index < curve1.Count; index++)
            {
                xAccDiff = xAccDiff + Math.Pow(curve1[index].X - curve2[index].X, 2);
                yAccDiff = yAccDiff + Math.Pow(curve1[index].Y - curve2[index].Y, 2);
                xAccDiffRev = xAccDiffRev + Math.Pow(curve1[index].X - curve2[curve1.Count - 1 - index].X, 2);
                yAccDiffRev = yAccDiffRev + Math.Pow(curve1[index].Y - curve2[curve1.Count - 1 - index].Y, 2);
            }
            var d1 = Math.Sqrt((xAccDiff + yAccDiff) / curve1.Count);
            var d2 = Math.Sqrt((xAccDiffRev + yAccDiffRev) / curve1.Count);
            return Math.Min(d1, d2);
        }

        internal void SetCurveSelection()
        {
            switch (MaxIdx)
            {
                case 0:
                    PointPair1.SelectedCurve = PointPair1.Curve1;
                    PointPair1.SelectedCenter = PointPair1.Center1;
                    PointPair2.SelectedCurve = PointPair2.Curve1;
                    PointPair2.SelectedCenter = PointPair2.Center1;
                    break;
                case 1:
                    PointPair1.SelectedCurve = PointPair1.Curve1;
                    PointPair1.SelectedCenter = PointPair1.Center1;
                    PointPair2.SelectedCurve = PointPair2.Curve2;
                    PointPair2.SelectedCenter = PointPair2.Center2;
                    break;
                case 2:
                    PointPair1.SelectedCurve = PointPair1.Curve2;
                    PointPair1.SelectedCenter = PointPair1.Center2;
                    PointPair2.SelectedCurve = PointPair2.Curve1;
                    PointPair2.SelectedCenter = PointPair2.Center1;
                    break;
                case 3:
                    PointPair1.SelectedCurve = PointPair1.Curve2;
                    PointPair1.SelectedCenter = PointPair1.Center2;
                    PointPair2.SelectedCurve = PointPair2.Curve2;
                    PointPair2.SelectedCenter = PointPair2.Center2;
                    break;
                default:
                    break;
            }
        }
    }
}
