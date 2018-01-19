using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class RDChannelViewModel
    {
        public RDChannelViewModel(RingdownTypeChannel model)
        {
            _model = model;
        }
        private readonly RingdownTypeChannel _model;
        public RingdownTypeChannel Model
        {
            get { return _model; }
        }
        public string PMU
        {
            get { return _model.PMU; }
        }
        public string Name
        {
            get { return _model.Name; }
        }
    }
}
