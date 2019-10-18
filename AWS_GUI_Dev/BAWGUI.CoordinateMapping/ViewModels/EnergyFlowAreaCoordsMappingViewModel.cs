using BAWGUI.CoordinateMapping.Models;
using BAWGUI.Core.Models;
using BAWGUI.Utilities;
using BAWGUI.Xml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BAWGUI.CoordinateMapping.ViewModels
{
    public class EnergyFlowAreaCoordsMappingViewModel : ViewModelBase
    {
        private EnergyFlowAreaCoordsMappingModel _model;

        public EnergyFlowAreaCoordsMappingViewModel()
        {
            _model = new EnergyFlowAreaCoordsMappingModel();
            _locations = new ObservableCollection<ConfigSite>();
        }

        public EnergyFlowAreaCoordsMappingViewModel(EnergyFlowAreaCoordsMappingModel area) : this()
        {
            _model = area;
            _locations = new ObservableCollection<ConfigSite>(_model.Locations);
        }
        public string AreaName 
        {
            get { return _model.AreaName; }
            set
            {
                try
                {
                    _model.AreaName = value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                OnPropertyChanged();
            }
        }
        public SignalMapPlotType Type 
        {
            get { return _model.Type; }
            set
            {
                _model.Type = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ConfigSite> _locations;
        public ObservableCollection<ConfigSite> Locations 
        {
            get { return _locations; }
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }
    }
}
