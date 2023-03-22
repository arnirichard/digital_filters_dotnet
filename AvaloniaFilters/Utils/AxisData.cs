using AvaloniaFilters.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaFilters
{
    public class AxisData
    {
        public double[] Values;
        // Range of Values
        public NumberRange<double> ActualRange;
        // Range of plot
        public NumberRange<double> VisibleRange;
        // 1-10
        public double LogBase;

        public AxisData(double[] values, NumberRange<double> actualRange, NumberRange<double> visibleRange, double scaleFactor)
        {
            Values = values;
            ActualRange = actualRange;
            VisibleRange = visibleRange;
            LogBase = scaleFactor;
        }
    }
}
