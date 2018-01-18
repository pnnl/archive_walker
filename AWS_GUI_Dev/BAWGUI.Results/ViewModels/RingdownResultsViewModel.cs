using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAWGUI.Results.ViewModels
{
    public class RingdownResultsViewModel
    {
        public RingdownResultsViewModel()
        {

        }
        private ObservableCollection<RingdownEventViewModel> _results;
    }
}
