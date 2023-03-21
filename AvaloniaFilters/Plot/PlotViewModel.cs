using AvaloniaFilters.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaFilters
{

    public class PlotViewModel
    {
        public double[] Y { get; init; }
        public NumberRange<double> YRange { get; init; }
        public double[]? X { get; }
        public NumberRange<double>? XRange { get; init; }

        public PlotViewModel(double[] y, double[]? x = null)
        {
            Y = y;
            YRange = new NumberRange<double>(y!.Min(), y!.Max());
            X = x;
            XRange = x != null
                ? new NumberRange<double>(x.Min(), x.Max())
                : null;
        }
    }
}
