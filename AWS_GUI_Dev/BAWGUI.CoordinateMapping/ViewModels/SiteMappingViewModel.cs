using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.CoordinateMapping.ViewModels
{
    public class SiteMappingViewModel
    {
        public SiteMappingViewModel()
        {
            DEFAreaSiteMappingVM = new EnergyFlowAreaCoordsMappingViewModel();
            SignalCoordsMappingVM = new SignalCoordsMappingViewModel();
        }
        public EnergyFlowAreaCoordsMappingViewModel DEFAreaSiteMappingVM { get; set; }
        public SignalCoordsMappingViewModel SignalCoordsMappingVM { get; set; }
    }
}
