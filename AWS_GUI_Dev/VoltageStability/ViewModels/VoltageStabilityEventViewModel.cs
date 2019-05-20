using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoltageStability.Models;

namespace VoltageStability.ViewModels
{
    public class VoltageStabilityEventViewModel
    {
        private VoltageStabilityEvent _model;
        public VoltageStabilityEventViewModel()
        {

        }
        public VoltageStabilityEventViewModel(VoltageStabilityEvent model)
        {
            _model = model;
            _subs.Clear();
            foreach (var ch in _model.Subs)
            {
                _subs.Add(new VSSubViewModel(ch));
            }

        }
        public string ID
        {
            get { return _model.ID; }
        }
        public string StartTime
        {
            get { return _model.Start; }
        }
        public string EndTime
        {
            get { return _model.End; }
        }
        public string Duration
        {
            get { return _model.Duration; }
        }

        public string NumberOfSubs
        {
            get { return _model.Subs.Count.ToString(); }
        }
        private List<VSSubViewModel> _subs = new List<VSSubViewModel>();
        public List<VSSubViewModel> Subs
        {
            get { return _subs; }
        }

    }
    public class VSSubViewModel
    {
        public VSSubViewModel(TheveninTypeSub model)
        {
            _model = model;
        }
        private TheveninTypeSub _model;
        public TheveninTypeSub Model
        {
            get { return _model; }
        }
        public string Name
        {
            get { return _model.Name; }
        }
    }

}
