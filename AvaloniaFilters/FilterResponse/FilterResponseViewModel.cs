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
    public class CanvasItem
    {
        public double Bottom { get; init; }
        public double Left { get; init; }
        public object Item { get; init; }

        public CanvasItem(double left, double bottom, object item)
        {
            Bottom = bottom;
            Left = left;
            Item = item;
        }
    }

    internal class FilterResponseViewModel : ViewModelBase
    {
        public int Fs { get; set; } = 10000;
        public CanvasItem[]? Zeros { get; set; }
        public CanvasItem[]? Poles { get; set; }
        public IIRFilter? Filter { get; set; }
        public PlotViewModel? Magnitude { get; set; }
        public PlotViewModel? Phase { get; set; }

        public void SetFilter(IIRFilter? filter)
        {
            Zeros = filter?.Zeros.Select(z => new CanvasItem((z.Real + 1) * 150, (z.Imaginary + 1)*150, z)).ToArray();
            Poles = filter?.Poles.Select(z => new CanvasItem((z.Real + 1) * 150, (z.Imaginary+ 1) * 150, z)).ToArray();
            Filter = filter;

            if (filter != null)
            {
                double[] omega = 0.1D.GetLinearRange(Math.PI, 300);
                double[] freqs = omega.Select(o => o * filter.Parameters.Fs / 2 / Math.PI).ToArray();
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
