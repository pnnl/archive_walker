using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.RunMATLAB.ViewModels;
using BAWGUI.Core;
using BAWGUI.Utilities;

namespace BAWGUI.Results.ViewModels
{
    public class FOOccurrenceChannelViewModel : ViewModelBase
    {
        public FOOccurrenceChannelViewModel(ForcedOscillationTypeOccurrenceChannel model)
        {
            _model = model;
        }
        private readonly ForcedOscillationTypeOccurrenceChannel _model;
        public ForcedOscillationTypeOccurrenceChannel Model
        {
            get { return _model; }
        }
        public string Name
        {
            get { return _model.Name; }
        }
        public float Amplitude
        {
            get
            {
                return _model.Amplitude;
            }
        }
        public float SNR
        {
            get
            {
                return _model.SNR;
            }
        }
        public float Coherence
        {
            get
            {
                return _model.Coherence;
            }
        }

    }
}
