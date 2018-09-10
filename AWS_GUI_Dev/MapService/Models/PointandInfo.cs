using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapService.Models
{


    public struct PointAndInfo
    {
        public PointLatLng Point;
        public string Info;
        public int ID;

        public PointAndInfo(PointLatLng point, int id, string info)
        {
            Point = point;
            Info = info;
            ID = id;
        }
    }
}
