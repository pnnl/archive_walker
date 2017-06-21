using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class OccurrenceViewModel:ViewModelBase
    {
        public OccurrenceViewModel(ForcedOscillationTypeOccurrence model)
        {
            _model = model;
        }
        private readonly ForcedOscillationTypeOccurrence _model;
        public ForcedOscillationTypeOccurrence Model
        {
            get { return _model; }
        }
        public string Start
        {
            get { return _model.Start; }
        }
        public string End
        {
            get { return _model.End; }
        }
        public string ID
        {
            get { return _model.OccurrenceID.ToString(); }
        }
        public string Frequency
        {
            get { return _model.Frequency.ToString(); }
        }
        public string Persistence
        {
            get { return _model.Persistence.ToString(); }
        }
    }
}
