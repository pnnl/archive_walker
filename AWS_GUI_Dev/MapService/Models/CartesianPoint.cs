using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapService.Models
{
    public class CartesianPoint
    {
        public CartesianPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Distance(CartesianPoint pt)
        {
            return Math.Sqrt(Math.Pow((pt.X - X), 2) + Math.Pow((pt.Y - Y), 2));
        }
    }
}
