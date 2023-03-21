using Avalonia.Controls;
using Filters;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaFilters
{
    internal class FilterResponseViewModel : ViewModelBase
    {
        public Complex[]? Zeros { get; set; }
        public Complex[]? Poles { get; set; }
        public IIRFilter? Filter { get; set; }
        public PlotViewModel? Magnitude { get; set; }
        public PlotViewModel? Phase { get; set; }

        public void SetFilter(IIRFilter filter)
        {
            Zeros = filter.Zeros;
            Poles = filter.Poles;
            Filter = filter;

            double[] omega = 0D.GetRange(Math.PI, 100);
            double[] freqs = omega.Select(o => o * filter.Fs / 2 / Math.PI).ToArray();
            Complex[] response = filter.GetResponse(omega);
            Magnitude = new PlotViewModel(response.Select(r => r.Magnitude).ToArray(),freqs);
            Phase = new PlotViewModel(response.Select(r => r.Phase).ToArray(), freqs);

            this.RaisePropertyChanged("Zeros");
            this.RaisePropertyChanged("Poles");
            this.RaisePropertyChanged("Filter");
            this.RaisePropertyChanged("Magnitude");
            this.RaisePropertyChanged("Phase");
        }
    }
}
