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
        public int Fs { get; set; } = 10000;
        public Complex[]? Zeros { get; set; }
        public Complex[]? Poles { get; set; }
        public IIRFilter? Filter { get; set; }
        public PlotViewModel? Magnitude { get; set; }
        public PlotViewModel? Phase { get; set; }

        public void SetFilter(IIRFilter? filter)
        {         
            Zeros = filter?.Zeros;
            Poles = filter?.Poles;
            Filter = filter;

            if (filter != null)
            {
                double[] omega = 0.1D.GetLinearRange(Math.PI, 300);
                double[] freqs = omega.Select(o => o * filter.Fs / 2 / Math.PI).ToArray();
                Complex[] response = filter.GetResponse(omega);
                Magnitude = new PlotViewModel(response.Select(r => Math.Log10(r.Magnitude)).ToArray(), freqs);
                double peak = response[0].Magnitude;
                double peakFreq = -1;
                for (int i = 0; i < response.Length; i++)
                {
                    if (response[i].Magnitude > peak)
                    {
                        peak = response[i].Magnitude;
                        peakFreq = freqs[i];
                    }

                }
                Phase = new PlotViewModel(response.Select(r => r.Phase).ToArray(), freqs);
            }
            else
            {
                Magnitude = Phase = null;
            }

            this.RaisePropertyChanged("Zeros");
            this.RaisePropertyChanged("Poles");
            this.RaisePropertyChanged("Filter");
            this.RaisePropertyChanged("Magnitude");
            this.RaisePropertyChanged("Phase");
        }
    }
}
