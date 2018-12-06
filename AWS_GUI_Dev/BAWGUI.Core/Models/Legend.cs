using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Core.Models
{
    public class Legend
    {
        public Legend(string n, OxyColor c)
        {
            Name = n;
            Color = c;
        }
        public string Name { get; set; }
        public OxyColor Color { get; set; }
    }
}
