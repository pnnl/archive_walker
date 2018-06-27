using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using BAWGUI.Utilities;
using Microsoft.VisualBasic;

namespace BAWGUI.SignalManagement.ViewModels
{
    public class PMUWithSamplingRate : ViewModelBase
    {
        public PMUWithSamplingRate(string pmu, int rate)
        {
            _pmu = pmu;
            _samplingRate = rate;
        }
        public PMUWithSamplingRate()
        {
        }
        private string _pmu;
        public string PMU
        {
            get
            {
                return _pmu;
            }
            set
            {
                _pmu = value;
                OnPropertyChanged();
            }
        }
        private int _samplingRate;
        public int SamplingRate
        {
            get
            {
                return _samplingRate;
            }
            set
            {
                _samplingRate = value;
                OnPropertyChanged();
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            PMUWithSamplingRate p = (PMUWithSamplingRate)obj;
            return this.PMU == p.PMU; // AndAlso Me.SamplingRate = p.SamplingRate
        }
        public override int GetHashCode()
        {
            // If item Is Nothing Then Return 0
            // Dim hashItemName = If(item.PMU Is Nothing, 0, item.PMU.GetHashCode())
            // Dim hashRate = item.SamplingRate.GetHashCode()
            return PMU.GetHashCode(); // Xor SamplingRate.GetHashCode()
        }
        public static bool operator ==(PMUWithSamplingRate x, PMUWithSamplingRate y)
        {
            return x.PMU == y.PMU; // AndAlso x.SamplingRate = y.SamplingRate
        }
        public static bool operator !=(PMUWithSamplingRate x, PMUWithSamplingRate y)
        {
            return x.PMU != y.PMU; // OrElse x.SamplingRate <> y.SamplingRate
        }
    }

    public class PMUWithSamplingRateComparer : IEqualityComparer<PMUWithSamplingRate>
    {
        public new bool Equals(PMUWithSamplingRate x, PMUWithSamplingRate y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;
            return (x.PMU == y.PMU); // AndAlso (x.SamplingRate = y.SamplingRate)
        }
        public new int GetHashCode(PMUWithSamplingRate item)
        {
            if (item == null)
                return 0;
            var hashItemName = item.PMU == null ? 0 : item.PMU.GetHashCode();
            // Dim hashRate = item.SamplingRate.GetHashCode()
            return hashItemName; // Xor hashRate
        }
    }
}
