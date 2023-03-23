using Avalonia.Controls;
using Filters;
using MathNet.Numerics;
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

        public override string ToString()
        {
            if (Item is Complex c)
            {
                return string.Format("{0}+i{1}",
                    c.Real.ToString("0.###"),
                    c.Imaginary.ToString("0.###"));
            }

            return Item?.ToString() ?? "null";
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
            Zeros = filter?.Zeros.Select(z => new CanvasItem(150 + z.Real * 125-5, 150+z.Imaginary * 125-5, z)).ToArray();
            Poles = filter?.Poles.Select(z => new CanvasItem(150 + z.Real * 125-5, 150+ z.Imaginary * 125-5, z)).ToArray();
            Filter = filter;

            if (filter != null)
            {
                double[] omega = 0.1D.GetLinearRange(Math.PI, 300);
                double[] freqs = omega.Select(o => o * filter.Parameters.Fs / 2 / Math.PI).ToArray();
                Complex[] response = filter.GetResponse(omega);
                Magnitude = new PlotViewModel(response.Select(r => 20*Math.Log10(r.Magnitude)).ToArray(), freqs);
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
